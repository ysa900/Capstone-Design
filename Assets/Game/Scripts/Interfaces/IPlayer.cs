//using UnityEngine;

public interface IPlayer
{
    void GetExp(int expAmount);
    public void RestoreHP(float restoreAmount);
    void TakeDamage(float damage);
    void OnPlayerBlinded();
}