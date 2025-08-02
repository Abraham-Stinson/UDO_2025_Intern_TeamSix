using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static int money;

    void Start()
    {

    }

    void Update()
    {

    }
    public void ReduceMoney(int amount)
    {
        money -= amount;
    }
}
