using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP;

public class QuanLyPhuongTien
{
    private List<PhuongTien> danhSachPhuongTien;

    public QuanLyPhuongTien()
    {
        danhSachPhuongTien = new List<PhuongTien>();
    }

    public void AddPhuongTien(PhuongTien phuongTien)
    {
        danhSachPhuongTien.Add(phuongTien);
    }

    public void DisplayAll()
    {
        foreach (PhuongTien phuongTien in danhSachPhuongTien)
        {
            Console.WriteLine(phuongTien.GetInfo());
            Console.WriteLine($"Giá lăn bánh: {phuongTien.TinhGiaLanBanh():N0} VNĐ");
            Console.WriteLine("----------------------------------------");
        }
    }

    public PhuongTien? FindMaxGiaLanBanh()
    {
        if (danhSachPhuongTien.Count == 0)
        {
            return null;
        }

        return danhSachPhuongTien
            .OrderByDescending(pt => pt.TinhGiaLanBanh())
            .First();
    }

    public List<PhuongTien> SearchByName(string tenHang)
    {
        return danhSachPhuongTien
            .Where(pt => pt.TenHang.Contains(tenHang, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}