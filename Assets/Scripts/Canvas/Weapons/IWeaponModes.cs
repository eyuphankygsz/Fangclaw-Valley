public interface IWeaponModes
{
    void Setup(Weapons weapon);
    void ExecuteMode();
    Weapons Weapon { get; set; }
}
