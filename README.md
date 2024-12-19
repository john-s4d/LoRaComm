# LoRaWAN Communication

This project implements a LoRaWAN communication server connecting to a Semtech UDP gateway, sending broadcast messages, and facilitating packet-based communication between endpoints.

## Features

- **Gateway Management**: Add and configure LoRaWAN gateways with custom power modes and transmission frequencies.
- **Packet Handling**: Handles LoRaWAN packet types like `PULL_DATA`, `PUSH_DATA`, and `TX_ACK` efficiently.
- **Broadcast System**: Supports scheduled broadcasting of messages to LoRaWAN endpoints.
- **Configurable Settings**: Reads gateway and broadcast configurations from a file.
- **Logging**: Logs all operations to a specified log file.

---

## Getting Started

### Prerequisites

- .NET runtime installed on your system.
- Configuration file (`config.txt`) in the expected format (see below for details).

### Running the Application

1. Clone the repository or download the project files.
2. Prepare a `config.txt` file with the following structure:
   ```
   gateways
   <GatewayID>,<PowerMode>,<Frequency1>,<Frequency2>,...
   
   broadcast
   <Message1>
   <Message2>
   ```
   Example:
   ```
   GATEWAYS
   gateway1,MAXIMUM,915.0,915.2,915.4
   gateway2,MINIMUM,915.0

   BROADCAST
   Hello, LoRa!
   Test Message
   ```

3. Optional: Use command-line arguments to specify custom paths for logs and configuration files:
   - `-l <LogFolder>`: Path to the log folder (default: `logs`).
   - `-c <ConfigFile>`: Path to the configuration file (default: `config.txt`).

---

## Usage

- Upon startup, the application reads the `config.txt` file to configure gateways and messages.
- It starts broadcasting messages at regular intervals.
- You can manually send packets via the console:
  - Type the data and press Enter.
  - Type `quit` to exit the application.

---

## Key Classes and Responsibilities

- **`LoRaComm`**:
  - Maintains gateway list and broadcasting messages.
  - Sends LoRaWAN packets, splitting them into manageable chunks if necessary.
  - Handles packet reception and acknowledgment.

- **`LoraWan`**:
  - Manages UDP communication.
  - Dispatches events for received packets.

- **`UdpClient`**:
  - Encapsulates UDP socket handling for asynchronous communication.

- **`Gateway`**:
  - Defines gateway properties like power mode and transmission frequencies.

---

## Logging

Logs are saved in the folder (default: `logs/log.txt`). They include:

- Information on gateway and packet events.
- Errors encountered during packet handling and broadcasting.