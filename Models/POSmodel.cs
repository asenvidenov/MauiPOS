﻿using SQLite;
using System.Numerics;

namespace MauiPOS.Models
{
    public class POSOps
    {
        [PrimaryKey]
        public int OpID { get; set; }
        public string OpName { get; set; }
        public string OpPass { get; set; }
        public int OpRole { get; set; }
        public string OpFName { get; set; }  
        public string OpCode { get; set; }
        public string OpApp { get; set; }
        public DateTime RoleEnd { get; set; }
    }

    public class POSOrders
    {
        [PrimaryKey, AutoIncrement]
        public int OrderID { get; set; }
        public int OpID { get; set; }
        public DateTime DateOpen { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DateClosed { get; set; }
        public int ObjectID { get; set; }
        public decimal CurrentSum { get; set; }
        public decimal FinalSum { get; set; }
        public Boolean RE { get; set; }
        public int REOrderID { set; get; }
    }

    public class POSCashPrice
    {
        [PrimaryKey]
        public int ID { get; set; }
        public int GoodsID { get; set; }
        public int ObjectID { get; set; }
        public int CashEnabled { get; set; }
        public int PosPrinter { get; set; }
        public decimal CashPrice { get; set; }
        public decimal DFPrice { get; set; }
    }

    public class POSDiscount
    {
        [PrimaryKey]
        public int ID { get; set; }
        [NotNull]
        public string DName { get; set; }
        [NotNull]
        public int DPercent { get; set; }
        [NotNull]
        public string DAccount { get; set; }
        [NotNull]
        public string DEnabled { get; set; }
        public string DOwner { get; set; }
        public DateTime DStart { get; set; }
        public DateTime DEnd { get; set; }
    }

    public class POSFP
    {
        [PrimaryKey]
        public string FP { get; set; }
        public string Prefix { get; set; }
        public string Limit { get; set; }
        public int PortSpeed { get; set; }
        public string Type { get; set; }
        public int ZReport { get; set; }
        public int ZBlocked { get; set; }
    }

    public class POSGoods
    {
        [PrimaryKey, NotNull]
        public int GoodsID { get; set; }
        [NotNull]
        public Boolean isGroup { get; set; }
        [NotNull]
        public int GParent { get; set; }
        public string CashName { get; set; }
        public string CashCode { get; set; }
        public Boolean DiscountAllowed { get; set; }
    }

    public class POSObjects 
    {
        [PrimaryKey]
        public int ObjectID { get; set; }
        [NotNull]
        public string ObjName { get; set; }
        [NotNull]
        public int Parent {  get; set; }
        public string ObjCode { get; set; }
        public Boolean isActive { get; set; }
    }

    public class POSOrderDetails
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [NotNull]
        public int OrderID { get; set; }
        [NotNull]
        public int OpID { get; set; }
        [NotNull]
        public int GoodsID { get; set; }
        [NotNull]
        public int Cnt {  get; set; }
        [NotNull]
        public Boolean Annul {  get; set; }
        public string Modiff { get; set; }
        [NotNull]
        public Decimal CashPrice { get; set; }
    }

    public class POSOrderChrono
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [NotNull]
        public int OrderID { get; set; }
        [NotNull]
        public int OpID { get; set; }
        [NotNull]
        public int GoodsID { get; set; }
        [NotNull]
        public int Cnt { get; set; }
        public string Modiff { get; set; }
        [NotNull]
        public DateTime Created { get; set; }
        [NotNull]
        public int OrderNum { get; set; }
        [NotNull]
        public Boolean Confirmed { get; set; }
        [NotNull]
        public Boolean Accepted { get; set; }
        [NotNull]
        public Boolean Printed { get; set; }
        public string OpName { get; set; }
        public string OpCode { get; set; }
        public int OpRole { get; set; }
    }

    public class POSSales
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [NotNull]
        public DateTime SaleDate { get; set; }
        [NotNull]
        public int OrderID { get; set; }
        [NotNull]
        public int OpID { get; set; }
        [NotNull]
        public decimal OrderSum { get; set; }
        [NotNull]
        public decimal CardSum { get; set; }
        [NotNull]
        public Decimal CashSum { get; set; }
        public Decimal DiscountSum { get; set; }
        public int DiscountPercent { get; set; }
        public int VAT { get; set; }
        public string FPN { get; set; }
        public string OpCode { get; set ; }
        public int SellNum { get; set; }
        public int RE { get; set; }
        public string opName { get; set; }
        public int opRole { get; set; }
        public Decimal VATAmount { get; set; }
        public string FlightNum { get; set; }
        public int RESellID { get; set; }
        public Boolean HasCopy { get; set; }
    }

    public class POSSalesPay
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [NotNull]
        public int SaleID { get; set; }
        public int Card { get; set; }
        public int Currency { get; set; }
        [NotNull]
        public Decimal Amount { get; set; }
        public Decimal CurrencyRate { get; set; }
    }

    public class sqlite_sequence
    {
        [PrimaryKey]
        public string? name { get; set; }
        public Int32 seq { get; set; }
    }
    public class ViewActiveOrders
    {
        public BigInteger OrderID { get; set; }
        public string? ObjName { get; set; }
        public decimal CurrentSum { set; get; }

    }
    

}
