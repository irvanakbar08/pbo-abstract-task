using System;

public interface Kemampuan
{
    void Gunakan(Robot target);
    int Cooldown();
    string Nama();
}

public abstract class Robot
{
    public string Nama { get; set; }
    public int Energi { get; set; }
    public int Armor { get; set; }
    public int Serangan { get; set; }

    protected bool ultimateSiap = false;

    public Robot(string nama, int energi, int armor, int serangan)
    {
        Nama = nama;
        Energi = energi;
        Armor = armor;
        Serangan = serangan;
    }

    public void Serang(Robot target)
    {
        int damage = Serangan - target.Armor;
        if (damage > 0)
        {
            target.Energi -= damage;
            Console.WriteLine($"{Nama} menyerang {target.Nama} dan menyebabkan {damage} kerusakan.");
        }
        else
        {
            Console.WriteLine($"{Nama} tidak dapat menembus armor {target.Nama}!");
        }

        if (target.Energi <= 0)
        {
            target.Mati();
        }
    }

    public abstract void GunakanUltimate(Robot target);

    public void GunakanKemampuan(Kemampuan skill, Robot target)
    {
        skill.Gunakan(target);
    }

    public void CetakInformasi()
    {
        Console.WriteLine($"Nama: {Nama} | Energi: {Energi} | Armor: {Armor} | Serangan: {Serangan}");
        Console.WriteLine("-------------------------");
    }

    public void Mati()
    {
        Console.WriteLine($"{Nama} telah dikalahkan!");
    }

    public void UltimateSiap()
    {
        ultimateSiap = true;
        Console.WriteLine($"\n{Nama} sudah siap menggunakan Ultimate!\n");
    }
}

public class BosRobot : Robot
{
    public BosRobot(string nama, int energi, int armor, int serangan)
        : base(nama, energi, armor, serangan) { }

    public override void GunakanUltimate(Robot target)
    {
        if (ultimateSiap)
        {
            Console.WriteLine($"{Nama} menggunakan Ultimate Plasma Cannon pada {target.Nama}!");
            target.Energi -= 50;
            if (target.Energi <= 0)
            {
                target.Mati();
            }
            ultimateSiap = false;
        }
        else
        {
            Console.WriteLine($"{Nama} belum bisa menggunakan Ultimate!");
        }
    }
}

public class RobotBiasa : Robot
{
    public RobotBiasa(string nama, int energi, int armor, int serangan)
        : base(nama, energi, armor, serangan) { }

    public override void GunakanUltimate(Robot target)
    {
        if (ultimateSiap)
        {
            Console.WriteLine($"{Nama} menggunakan Ultimate Sword Slash pada {target.Nama}!");
            target.Energi -= 40;
            if (target.Energi <= 0)
            {
                target.Mati();
            }
            ultimateSiap = false;
        }
        else
        {
            Console.WriteLine($"{Nama} belum bisa menggunakan Ultimate!");
        }
    }
}

public class Repair : Kemampuan
{
    private int cooldown = 2;
    private int lastUsed = -2;

    public void Gunakan(Robot target)
    {
        if (CoolDownReady())
        {
            target.Energi += 20;
            Console.WriteLine($"{target.Nama} menggunakan Repair dan memulihkan energinya menjadi {target.Energi}");
            lastUsed = Program.Turn;
        }
        else
        {
            Console.WriteLine($"{target.Nama} tidak dapat menggunakan Repair karena masih dalam cooldown ({Cooldown() - (Program.Turn - lastUsed)} giliran tersisa).");
        }
    }

    public int Cooldown() => cooldown;

    public string Nama() => "Repair";

    private bool CoolDownReady()
    {
        return Program.Turn - lastUsed >= cooldown;
    }
}

public class ElectricShock : Kemampuan
{
    private int cooldown = 3;
    private int lastUsed = -3;

    public void Gunakan(Robot target)
    {
        if (CoolDownReady())
        {
            target.Energi -= 30;
            Console.WriteLine($"{target.Nama} terkena Electric Shock dan energinya tersisa {target.Energi}");
            lastUsed = Program.Turn;
        }
        else
        {
            Console.WriteLine($"Electric Shock masih dalam cooldown ({Cooldown() - (Program.Turn - lastUsed)} giliran tersisa).");
        }
    }

    public int Cooldown() => cooldown;

    public string Nama() => "Electric Shock";

    private bool CoolDownReady()
    {
        return Program.Turn - lastUsed >= cooldown;
    }
}

public class Program
{
    public static int Turn { get; private set; } = 0;

    public static void Main(string[] args)
    {
        RobotBiasa robotAlpha = new RobotBiasa("Alpha", 100, 10, 25);
        BosRobot bosBalmond = new BosRobot("Bos Balmond", 150, 20, 30);

        Repair repair = new Repair();
        ElectricShock electricShock = new ElectricShock();

        Console.WriteLine("=== Informasi Awal ===");
        robotAlpha.CetakInformasi();
        bosBalmond.CetakInformasi();

        while (robotAlpha.Energi > 0 && bosBalmond.Energi > 0)
        {
            Console.WriteLine("\n=== Pertarungan ===");
            robotAlpha.CetakInformasi();
            bosBalmond.CetakInformasi();

            robotAlpha.Serang(bosBalmond);
            if (bosBalmond.Energi > 0) bosBalmond.Serang(robotAlpha);

            if (Turn % 5 == 0)
            {
                robotAlpha.UltimateSiap();
                bosBalmond.UltimateSiap();
            }

            if (Turn % 2 == 0) robotAlpha.GunakanKemampuan(repair, robotAlpha);
            if (Turn % 3 == 0) bosBalmond.GunakanKemampuan(electricShock, robotAlpha);

            if (Turn % 7 == 0)
            {
                robotAlpha.GunakanUltimate(bosBalmond);
                bosBalmond.GunakanUltimate(robotAlpha);
            }

            robotAlpha.CetakInformasi();
            bosBalmond.CetakInformasi();
            Turn++;
        }

        Console.WriteLine("\nPertarungan selesai.");
    }
}
