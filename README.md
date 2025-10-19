# MoviesSearch Application

## Overview
MoviesSearch is a Blazor-based web application that allows users to search for movies using the OMDb API, view detailed movie information, and access recent search history. The application follows **Clean Architecture** principles to ensure maintainability, testability, and scalability. For simplicity, authentication has not been implemented, but comments in the code indicate where authentication logic would be applied. In a production environment, a logging system should be added to track application behavior and errors.

## Motivation for Clean Architecture
Clean Architecture, was chosen for the following reasons:

- **Separation of Concerns**: Layers (Core, Application, Infrastructure, Presentation) have distinct responsibilities, making the codebase easier to understand and maintain.
- **Testability**: Core business logic and use cases are isolated from external systems (e.g., OMDb API, UI), enabling comprehensive unit testing.
- **Flexibility**: Allows swapping implementations (e.g., replacing `InMemorySearchHistoryRepository` with a database) without affecting business logic.
- **Scalability**: Clear layer boundaries facilitate adding new features with minimal impact.
- **Maintainability**: Low coupling reduces the risk of changes breaking unrelated components.

## Benefits of Clean Architecture
- **Decoupled Dependencies**: The Core layer (entities and interfaces) has no dependencies, keeping business rules framework-agnostic.
- **Ease of Maintenance**: Changes to the UI (Blazor in `Home.razor`) or data source (OMDb API via `OmdbClient`) don’t affect business logic.
- **Reusability**: Entities (`Movie`, `MovieDetails`) and use cases are reusable across different frontends or data sources.
- **Robustness**: Interfaces (`IMovieRepository`, `ISearchHistoryRepository`) enforce contracts, reducing integration errors.

## Solution Structure
The solution is organized into four projects, following Clean Architecture layers:

1. **MoviesSearch.Core**:
   - Contains entities (`Movie`, `MovieDetails`) and interfaces (`IMovieRepository`, `ISearchHistoryRepository`).
   - Defines business models and contracts, independent of external systems.
   - Example: `MovieDetails` includes `Id` (random GUID), `ImdbRating`, `Title`, `Plot`, etc.

2. **MoviesSearch.Application**:
   - Implements use cases (`GetLatestSearches`, `GetMovieDetails`, `SearchMovies`) that orchestrate business logic.
   - Coordinates interactions between repositories and entities.
   - Example: `SearchMovies` calls `IMovieRepository.SearchMoviesAsync` and saves searches via `ISearchHistoryRepository`.

3. **MoviesSearch.Infrastructure**:
   - Provides implementations like `OmdbClient` (for OMDb API) and `InMemorySearchHistoryRepository` (for search history).
   - Handles external concerns like HTTP requests and JSON deserialization.
   - Example: `OmdbClient` uses case-insensitive deserialization to map `imdbRating` to `ImdbRating`.

4. **MoviesSearch.Presentation**:
   - Contains the Blazor frontend (`Home.razor`) for user interaction.
   - Integrates with use cases to display search results and movie details.

5. **MoviesSearch.Tests**:
   - Contains unit tests .
   - Uses `xunit`, `Moq`, and `FluentAssertions` for testing.

## Main Workflow
The application’s workflow is as follows:

1. **User Interaction (Presentation Layer)**:
   - In `Home.razor`, the user enters a search term (e.g., "Matrix").
   - The UI calls `Search` to fetch movie results.
   - Selecting a movie (e.g., IMDb ID "tt0133093") triggers `GetMovieDetails` to show details.
   - Recent searches are displayed via `GetLatestSearches`.
   - *Note*: Authentication is not implemented, but comments in `Home.razor` and use cases indicate where checks (e.g., user authorization) would be added.

2. **Business Logic (Application Layer)**:
   - `SearchMovies.ExecuteAsync`:
     - Calls `IMovieRepository.SearchMoviesAsync` to fetch movies.
     - Saves the search term using `ISearchHistoryRepository.AddSearchAsync`.
   - `GetMovieDetails.ExecuteAsync`:
      - Calls `IMovieRepository.GetMovieDetailsAsync` with `CancellationToken` support.
      - Validates the IMDb ID.
   - `GetLatestSearches.ExecuteAsync`:
     - Retrieves recent search terms from `ISearchHistoryRepository`.

3. **Data Access (Infrastructure Layer)**:
   - `OmdbClient`:
     - Makes HTTP requests to the OMDb API using `HttpClient`.
     - Deserializes JSON responses into `Movie` and `MovieDetails`, mapping `imdbRating` to `ImdbRating` using case-insensitive deserialization.
     - Assigns random GUIDs to `Id` properties with `Guid.NewGuid()`.
     - *Note*: Comments in `OmdbClient` indicate where authentication (e.g., API key validation) could be added.
   - `InMemorySearchHistoryRepository`:
     - Stores search terms in memory (no `CancellationToken` support).
     - Returns recent searches in reverse chronological order.

4. **Response to User (Presentation Layer)**:
   - Search results (`Movie`) are displayed as a list in `Home.razor`.
   - Movie details (`MovieDetails`) are shown with formatted text (e.g., `Plot` wrapped via `.detail-plot`).
   - Recent searches are displayed for quick access.

## Getting Started
### Prerequisites
- .NET 9.0 SDK
- A valid OMDb API key (configure in `MoviesSearch.Presentation/appsettings.json`)

### Setup
1. **Clone the Repository**:
   ```bash
   git clone <repository-url>
   cd MoviesSearch
2. **Configure API key**:
   ```json
    {
        "Omdb": {
        "ApiKey": "<your-real-api-key>",
        "BaseUrl": "http://www.omdbapi.com/"
        }
    }