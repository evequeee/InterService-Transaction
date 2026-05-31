# InterService Transaction

Implementation of a distributed transaction across three microservices using the Orchestration pattern.

## Tech Stack
* **Platform:** .NET 10
* **Message Broker:** RabbitMQ + MassTransit (v8, Open-Source)
* **Inter-service Communication:** gRPC
* **Entry Point:** ASP.NET Core Web API (REST)

## Architecture & Transaction Flow
1. **ServiceA (Orchestrator):** Receives a synchronous REST request (`POST /api/transaction/start`).
2. **Asynchronous Step:** **ServiceA** concurrently sends messages via RabbitMQ to its own consumer and to **ServiceB**, awaiting responses (using the *Request/Response* pattern).
3. **Synchronous Step:** Upon receiving successful responses from both services, **ServiceA** performs a synchronous gRPC call to **ServiceC**.
4. **Error Handling:** If any internal service is unavailable or times out, the transaction is aborted, and the client receives a clean `504 Gateway Timeout` response instead of a system crash.

## Testing the API
To verify the successful flow,a POST request is used:

http://localhost:5242/api/transaction/start