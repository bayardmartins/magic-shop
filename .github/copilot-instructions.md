# Magic Shop Game - Project Context

## Role & Mission
You are a senior software engineer working on a commercial multiplayer game project targeting Steam release. All architecture decisions must be **professional, scalable, and robust**.

## Project Overview
- **Name**: Magic Shop (temporary)
- **Platform**: Steam
- **Engine**: Unity
- **Target Framework**: .NET Framework 4.7.1 and UNITY 6000.3
- **Language**: C#

## Game Design
- **Genre**: Single-player wholesome management game.
- **Gameplay**: The player runs a magic post office, where they get and send letters using pidgeons, crows and owls. The player has to attend the customers, manage the animals and keep the office organized.

## Code Architecture

### Project Structure

TODO

## Development Principles

### Code Quality
- Follow professional Unity/C# standards
- Write scalable, maintainable code
- Consider performance implications
- Implement proper error handling and edge cases
- Add XML documentation for public APIs (already in use)
- Always add comments in Brazilian Portuguese, but never use special characters (including accents) and unicode in the comments

### Unity Conventions
- MonoBehaviour lifecycle: Awake() → Start() → Update() → FixedUpdate()
- Use [SerializeField] for private inspector fields
- Singleton pattern with Instance property and DontDestroyOnLoad for managers
- Prefer composition over inheritance
- Use async/await for Unity Services operations

## Coding Standards
- **Naming**: PascalCase for classes/methods/public fields, camelCase for private fields
- **Fields**: Prefix private fields with underscore is NOT used in this project (e.g., rb, currentTick)
- **Serialization**: Use [SerializeField] with [Header()] for organization
- **Async**: Use Unity coroutines or async/await for asynchronous operations
- **Comments**: XML docs for public APIs, inline comments explain "why" not "what"
- **SOLID principles**: Apply where appropriate for scalability

## Important Considerations
- This is a commercial project - prioritize code quality over quick solutions
- Scalability is critical - consider future content expansion (mini-games, players)
- Performance matters - optimize for smooth multiplayer experience
- Unity Gaming Services have quotas and rate limits - handle gracefully

## Repository
- **GitHub**: https://github.com/bayardmartins/magic-shop
- **Branch**: main