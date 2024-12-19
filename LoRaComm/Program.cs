class Program
{

    private const string DEFAULT_LOG_FOLDER = @"logs";
    private const string DEFAULT_CONFIG_FILE = @"config.txt";
    private const string LOG_FILENAME = "log.txt";
    private const int BROADCAST_INTERVAL = 666 * 1000;

    private static LoRaComm _loraServer;
    private static Logger _logger;

    static void Main(string[] args)
    {
        Dictionary<string, string> parameters = ParseArgs(args);

        string logFolder = parameters.ContainsKey("l") ? parameters["l"] : DEFAULT_LOG_FOLDER;
        string configFile = parameters.ContainsKey("c") ? parameters["c"] : DEFAULT_CONFIG_FILE;

        _logger = new Logger($@"{DEFAULT_LOG_FOLDER}\{LOG_FILENAME}");
        _logger.LogEvent += _logger_LogEvent; ;

        _loraServer = new LoRaComm(_logger);

        ReadConfiguration(configFile);

        _loraServer.Start();

        _loraServer.StartBroadcast(BROADCAST_INTERVAL);

        Run().Wait();

        _loraServer.Stop();
    }

    private static void ReadConfiguration(string filePath)
    {
        const string GATEWAYS = "gateways";
        const string BROADCAST = "broadcast";

        string[] lines = File.ReadAllLines(filePath);

        string currentSection = string.Empty;

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.ToLower() == GATEWAYS || line.ToLower() == BROADCAST)
            {
                currentSection = line.ToLower();
                continue;
            }

            if (currentSection == GATEWAYS)
            {
                var tokens = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                string gatewayId = tokens[0];
                TxPowerMode txPowerMode = (TxPowerMode)Enum.Parse(typeof(TxPowerMode), tokens[1]);
                List<float> channels = tokens.Skip(2).Select(x => float.Parse(x)).ToList();

                _loraServer.AddGateway(gatewayId, txPowerMode, channels.ToArray());
            }

            if (currentSection == BROADCAST)
            {
                _loraServer.AddBroadcast(line);
            }
        }
    }

    private static void _logger_LogEvent(object sender, LogEventArgs e)
    {
        if (e.Level <= LogLevel.INFO)
        {
            Console.WriteLine(e.Level.ToString().PadRight(6, ' ') + " | " + e.Message);
        }
    }

    private static Dictionary<string, string> ParseArgs(string[] args)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        for (int i = 0; i < args.Length - 1; i = i + 2)
        {
            if (args[i].StartsWith("-"))
            {
                // TODO: Args can only be in a predefined set of characters

                result.Add(args[i].Substring(1), args[i + 1]);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        return result;
    }

    private async static Task Run()
    {
        Console.WriteLine("Enter data to send (type 'quit' to exit):");

        while (true)
        {
            string input = await Task.Run(() => Console.ReadLine());

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Input is empty. Please enter valid data.");
                continue;
            }

            if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Exiting...");
                break;
            }

            try
            {
                _loraServer.SendLoraWanPacket(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

}
