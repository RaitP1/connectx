namespace Domain;

public sealed record PlayerType(bool IsAI, AIDifficulty? Difficulty = null);
