# .NET Microservices Project

## Overview
This project is a microservices-based backend system built with .NET and ASP.NET Core. 
The backend provides core functionalities such as item cataloging, inventory management, identity handling, and trading mechanisms.

## Architecture
The system follows a microservices architecture with the following key services:
- **Catalog Service**: Manages the list of items available for purchase.
- **Inventory Service**: Keeps track of the items owned by a user.

### Key Features
- Independent databases for each service to ensure data isolation.
- Message broker (RabbitMQ) for asynchronous communication.
- API Gateway for client interaction.
- It can be extended to also do logging, distributed tracing, and monitoring for system health and debugging.

## Tech Stack
- **Backend**: .NET 9, ASP.NET Core, MongoDB
- **Message Broker**: RabbitMQ with MassTransit
- **Development Tools**: Visual Studio Code, Docker, .NET CLI

## Setup & Installation
1. Install the following tools:
   - .NET 9 SDK
   - Docker Desktop
   - Visual Studio Code
2. Clone the repository and navigate to the project directory.
3. Run the required infrastructure services using Docker.
4. Use .NET CLI to build and run the microservices.

## Objectives
Demonstrate knowledge in:
- Microservices architecture.
- Showcase building independent, deployable services.
- Implement inter-service communication using message brokers.
- Use containerization for running infrastructure services.

## Future Enhancements
- Include unit tests and integration tests
- Build a front-end that users can interract with
- Implement additional security measures for authentication and authorization.
- Optimize inter-service communication and scalability.
