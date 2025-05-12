using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private Dictionary<WeaponType, int> weaponLevels = new();
    private WeaponType equippedWeapon;

    public void InitWeapons()
    {
        // 기본 무기 지급
        if (!weaponLevels.ContainsKey(WeaponType.Pistol))
        {
            weaponLevels[WeaponType.Pistol] = 1;
            equippedWeapon = WeaponType.Pistol;
        }
    }

    public bool UnlockWeapon(WeaponType type, int cost)
    {
        if (weaponLevels.ContainsKey(type)) return false;
        if (!GameManager.Instance.SaveManager.TrySpendMoney(cost)) return false;

        weaponLevels[type] = 1;
        return true;
    }

    public bool UpgradeWeapon(WeaponType type, int cost)
    {
        if (!weaponLevels.ContainsKey(type)) return false;
        if (!GameManager.Instance.SaveManager.TrySpendMoney(cost)) return false;

        weaponLevels[type]++;
        return true;
    }

    public int GetWeaponLevel(WeaponType type) => weaponLevels.TryGetValue(type, out var lvl) ? lvl : 0;
    public WeaponType GetEquippedWeapon() => equippedWeapon;

    public void EquipWeapon(WeaponType type)
    {
        if (weaponLevels.ContainsKey(type))
        {
            equippedWeapon = type;
        }
    }
}

public enum WeaponType
{
    Pistol,
    Shotgun,
    AssaultRifle,
    Flamethrower,
    LaserGun
}

public struct WeaponData
{
    public float fireInterval;
    public int damage;
    public int bulletCount;
    public float spreadAngle;
}