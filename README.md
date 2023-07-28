# Hacker News API Proxy

This project serves as a proxy for the Hacker News API. It provides an efficient means to retrieve the details of the first 'n' best stories from the Hacker News API, where 'n' is specified by the caller to the API.

## How to Run the Application

1. Clone the repository: `git clone https://github.com/piotr-nawrot/HackerNewsProxy.git`
2. Navigate to the project directory: `cd HackerNewsProxy`
3. Run the application: `dotnet run`
4. Swagger is available at `http://localhost:5053/swagger/index.html`
5. The API can also be tested with the following CURL command: `curl -X 'GET' 'http://localhost:5053/v1/stories?top=5' -H 'accept: application/json'`

## Assumptions

1. This project is designed for demo purposes. As such, the endpoints are currently served without security measures and with a permissive CORS policy. This is to prevent irrelevant errors for the task reviewer. Please note: in a production environment, the endpoints would need to be properly secured and the CORS policy should be as restrictive as necessary to prevent unauthorized access.
2. The Hacker News API is consistently available and reliable.
3. 'n' stories requested will always be available.
4. The cache duration for stories has been set to 1 minute to prevent overloading the Hacker News API.
5. The purpose of the task was the creation of a microservice. Strict coding standards are assumed and therefore followed best practices for project boilerplate, code analyzers, etc. These configurations come from Microsoft related sources.
6. I wanted to demonstrate some good practices, but due to the nature of the task, they are proof of concept (PoC).
7. I assumed that the number of requested stories should be between 1 and 500 inclusive, 500 is the maximum number of stories returned by the Hacker News API according to their documentation.
8. Assuming that this is the creation of a new microservice, I chose minimalistic project names for namespace simplification. No library project is intended to be shared as NuGet with other systems.

## Implementation

1. Stories are cached for an minute to ensure efficient servicing of large numbers of requests.
2. Included sorting of stories by score in a descending order.
3. Exception handling middleware and logging have been implemented as a proof of concept (PoC) - this is not a production-ready implementation.
4. Example of unit tests implemented using xUnit, Moq, FluentAssertions.
5. Project analyzers are configured to be strict to catch as many errors and ill practices at compilation time as possible.
6. Used modern, efficient libraries for code generation like Mapster for mapping and Refit for generating API client code.
7. Classes are sealed to make clear that they are not to be inherited with the current design. Micro-optimization is a nice side effect, but not the purpose.

## Potential Enhancements

1. Implement a mechanism to handle API unavailability or rate limiting by the Hacker News API.
2. Add more comprehensive error handling and logging.
3. Introduce a persistence layer (like a database) for longer-term caching and resilience.
4. More comprehensive OpenApi descriptions with full documentation of API contracts and examples.
5. Add a structured HTTP file.
6. Replace in-memory cache with a shared distributed one like Redis.
7. Containerize the application.
8. Add more tests with dynamic test data generation.
9. Better error and retry handling thanks to the Circuit Breaker pattern / Polly.NET and/or request queue. This would improve the system's resilience and ability to cope with potential issues.
10. Better organization of request validation with FluentValidation.
11. Next step from minimal API could be to Request-Endpoint-Response with FastEndpoints NuGet. It could help further increase maintainability.
12. Automatic generic DI registration.
13. Ensure compatibility with AOT compilation which will be generally available with .NET 8 and provides great resource efficiency improvements.
14. Tests implementation can further be simplified with automocking libraries. If this solution would be in a production context, additional integration and performance tests could be created.
15. Implementation of telemetry and observability for continuous service monitoring.
16. HealthCheck endpoints could be added for better service monitoring and maintenance.