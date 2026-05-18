using Domain;

namespace ConnectX.TestHelpers.Builders;

public static class ConfigBuilder
{
    public static GameConfig Standard(string name = "Test") => new(
        Name: name,
        Rows: 6,
        Columns: 7,
        WinCondition: 4,
        Player1Name: "Player 1",
        Player1Symbol: "X",
        Player2Name: "Player 2",
        Player2Symbol: "O",
        Topology: EBoardTopology.Rectangle,
        Player1Type: new PlayerType(false),
        Player2Type: new PlayerType(false));

    public static GameConfig Cylinder(string name = "Cylinder") => Standard(name) with
    {
        Topology = EBoardTopology.Cylinder
    };

    public static GameConfig WithAI(string name = "AI-Game") => Standard(name) with
    {
        Player2Type = new PlayerType(true, EAIDifficulty.Hard)
    };
}
