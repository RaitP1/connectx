using Domain;

namespace Application.AI;

public interface IAIPlayer
{
    int GetMove(GameBrain brain, int player);
}
