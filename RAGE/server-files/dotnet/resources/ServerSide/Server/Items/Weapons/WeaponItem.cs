//using ServerSide.Characters;
//using System;

//public class WeaponItem : Item
//{
//    public int MaxAmmo { get; set; }
//    public int CurrentAmmo { get; set; }

//    public WeaponItem(Character character, string name, int maxAmmo)
//        : base(character, name, "weapon", 1, false)
//    {
//        MaxAmmo = maxAmmo;
//        CurrentAmmo = 0;
//    }

//    public bool AddAmmo(int amount)
//    {
//        if (CurrentAmmo + amount > MaxAmmo) return false;
//        CurrentAmmo += amount;
//        return true;
//    }

//    public int RemoveAmmo(int amount)
//    {
//        int actualAmount = Math.Min(amount, CurrentAmmo);
//        CurrentAmmo -= actualAmount;
//        return actualAmount;
//    }
//}