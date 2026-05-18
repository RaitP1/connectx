namespace Domain;

public sealed record PlayerType(bool IsAI, EAIDifficulty? Difficulty = null);
