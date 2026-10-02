using OOP;
using System;
using System.Collections.Generic;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("==========================================");
Console.WriteLine("   BÀI KIỂM TRA SỐ 01 - PHẦN OOP");
Console.WriteLine("   QUẢN LÝ PHƯƠNG TIỆN AUTOSPEED");
Console.WriteLine("==========================================");


// ==================================================
// TC01 - Kiểm tra Validation Năm sản xuất
// ==================================================

Console.WriteLine("\n===== TC01: KIỂM TRA NĂM SẢN XUẤT =====");

try
{
    OTo otoTC01 = new OTo(
        "OT001",
        "Toyota",
        1850,
        1000000000m,
        5,
        2.0
    );

    Console.WriteLine("TC01: KHÔNG ĐẠT - Không phát hiện lỗi.");
}
catch (ArgumentException ex)
{
    Console.WriteLine("TC01: ĐẠT");
    Console.WriteLine($"Đã bắt được ArgumentException: {ex.Message}");
}


// ==================================================
// TC02 - Kiểm tra tính giá lăn bánh ô tô
// ==================================================

Console.WriteLine("\n===== TC02: TÍNH GIÁ LĂN BÁNH Ô TÔ =====");

OTo otoTC02 = new OTo(
    "OT002",
    "Toyota",
    2025,
    1000000000m,
    5,
    2.0
);

decimal giaLanBanhOTo = otoTC02.TinhGiaLanBanh();

Console.WriteLine(otoTC02.GetInfo());
Console.WriteLine($"Giá lăn bánh: {giaLanBanhOTo:N0} VNĐ");

if (giaLanBanhOTo == 1420000000m)
{
    Console.WriteLine("TC02: ĐẠT");
}
else
{
    Console.WriteLine("TC02: KHÔNG ĐẠT");
}


// ==================================================
// TC03 - Kiểm tra tính giá lăn bánh xe máy
// ==================================================

Console.WriteLine("\n===== TC03: TÍNH GIÁ LĂN BÁNH XE MÁY =====");

XeMay xeMayTC03 = new XeMay(
    "XM001",
    "Honda",
    2025,
    50000000m,
    150
);

decimal giaLanBanhXeMay = xeMayTC03.TinhGiaLanBanh();

Console.WriteLine(xeMayTC03.GetInfo());
Console.WriteLine($"Giá lăn bánh: {giaLanBanhXeMay:N0} VNĐ");

if (giaLanBanhXeMay == 51000000m)
{
    Console.WriteLine("TC03: ĐẠT");
}
else
{
    Console.WriteLine("TC03: KHÔNG ĐẠT");
}


// ==================================================
// TC04 - Kiểm tra đa hình List<PhuongTien>
// ==================================================

Console.WriteLine("\n===== TC04: KIỂM TRA ĐA HÌNH =====");

List<PhuongTien> danhSach = new List<PhuongTien>();

danhSach.Add(otoTC02);
danhSach.Add(xeMayTC03);

foreach (PhuongTien phuongTien in danhSach)
{
    Console.WriteLine(phuongTien.GetInfo());
    Console.WriteLine(
        $"Giá lăn bánh: {phuongTien.TinhGiaLanBanh():N0} VNĐ"
    );
    Console.WriteLine("------------------------------------------");
}

Console.WriteLine("TC04: ĐẠT - List<PhuongTien> chứa cả OTo và XeMay.");


// ==================================================
// TC05 - Tìm giá lăn bánh lớn nhất
// ==================================================

Console.WriteLine("\n===== TC05: TÌM GIÁ LĂN BÁNH LỚN NHẤT =====");

QuanLyPhuongTien quanLy = new QuanLyPhuongTien();

quanLy.AddPhuongTien(otoTC02);
quanLy.AddPhuongTien(xeMayTC03);

PhuongTien? phuongTienMax = quanLy.FindMaxGiaLanBanh();

if (phuongTienMax != null)
{
    Console.WriteLine("Phương tiện có giá lăn bánh lớn nhất:");
    Console.WriteLine(phuongTienMax.GetInfo());
    Console.WriteLine(
        $"Giá lăn bánh: {phuongTienMax.TinhGiaLanBanh():N0} VNĐ"
    );

    if (phuongTienMax.TinhGiaLanBanh() == 1420000000m)
    {
        Console.WriteLine("TC05: ĐẠT");
    }
    else
    {
        Console.WriteLine("TC05: KHÔNG ĐẠT");
    }
}
else
{
    Console.WriteLine("TC05: KHÔNG ĐẠT - Danh sách rỗng.");
}


Console.WriteLine("\n==========================================");
Console.WriteLine("        HOÀN THÀNH KIỂM TRA OOP");
Console.WriteLine("==========================================");

Console.WriteLine("\nNhấn phím bất kỳ để kết thúc...");
Console.ReadKey();