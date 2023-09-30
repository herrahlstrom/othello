namespace Othello.Engine;

public interface ISerialiable
{
    void Load(string data);

    string Serialize();
}
