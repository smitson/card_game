using UnityEngine;

public class Card
{
    private Vector3 position;

    public Card(Vector3 position)
    {
        this.position = position;
    }

    public Vector3 GetPosition()
    {
        return position;
    }
}
