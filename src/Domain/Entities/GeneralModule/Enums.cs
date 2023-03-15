using System.ComponentModel;

namespace Domain.Entities.GeneralModule
{
    public enum SufficientFor
    {
        [Description("1 Person")]
        SoloPerson = 1,
        [Description("2 People")]
        TwoPeople = 2,
        [Description("Family of 4")]
        FamilyOfFour = 3,
    }
    public enum ItemsType
    {
        [Description("Fruit and Veg")]
        FruitAndVeg = 1,
        [Description("Fruit Only")]
        FruitOnly = 2,
        [Description("Veg Only")]
        VegOnly = 3,
    }
    public enum ItemType
    {
        [Description("Fruit")]
        Fruit = 1,
        [Description("Vegetable")]
        Vegetable = 2,
    }
    public enum ItemUnit
    {
        [Description("Kg")]
        Kg = 1,
        [Description("g")]
        Gram = 1,
        [Description("X")]
        Unit = 2,
        [Description("Bunch")]
        Bunch = 3,
        [Description("Dozen")]
        Dozen = 3,
    }
    public enum SubscriptionState
    {
        [Description("Active")]
        Active = 1,
        [Description("Paused")]
        Paused = 2,
        [Description("Cancelled")]
        Cancelled = 3,
        [Description("Future")]
        Future = 4,
        [Description("False")]
        False = 5,
        [Description("InTrial")]
        InTrial = 6,
        [Description("NonRenewing")]
        NonRenewing = 7,
    }
    public enum PantryItemFrequency
    {
        [Description("Just Once")]
        JustOnce = 1,
        [Description("Every Box")]
        EveryBox = 2,
        [Description("Every Second Box")]
        EverySecondBox = 3,
    }
    public enum ShippingFrequency
    {
        [Description("OneOff")]
        OneOff = 1,
        [Description("Weekly")]
        Weekly = 2,
        [Description("Fortnightly")]
        Fortnightly = 3,
    }
}
