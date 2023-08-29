using System.ComponentModel;

namespace Domain.Entities.GeneralModule;

public enum BoxSize
{
    [Description("Small")] Small = 1,
    [Description("Medium")] Medium = 2,
    [Description("Large")] Large = 3,
}
public enum SufficientFor
{
    [Description("1 Person")] SoloPerson = 1,
    [Description("2 People")] TwoPeople = 2,
    [Description("Family of 4")] FamilyOfFour = 3,
}
public enum BannerColor
{
    [Description("Primary")] Primary = 1,
    [Description("Secondary")] Secondary = 2,
    [Description("Info")] Info = 3,
    [Description("Warning")] Warning = 4,
    [Description("Danger")] Danger = 5,
}
public enum ProduceType
{
    [Description("Fruit")] Fruit = 1,
    [Description("Vegetable")] Vegetable = 2,
}
public enum ItemsType
{
    [Description("Fruit and Veg")] FruitAndVeg = 1,
    [Description("Fruit Only")] FruitOnly = 2,
    [Description("Veg Only")] VegOnly = 3,
}
public enum ItemType
{
    [Description("Produce (fruits and vegetables)")] Produce = 1,
    [Description("Meat and poultry")] MeatAndPoultry = 2,
    [Description("Seafood")] Seafood = 3,
    [Description("Dairy and eggs")] DairyAndEggs = 4,
    [Description("Bakery items")] BakeryItems = 5,
    [Description("Canned and jarred goods")] CannedAndJarredGoods = 6,
    [Description("Frozen foods")] FrozenFoods = 7,
    [Description("Snacks and chips")] SnacksAndChips = 8,
    [Description("Beverages")] Beverages = 9,
    [Description("Condiments and sauces")] CondimentsAndSauces = 10,
    [Description("Spices and seasonings")] SpicesAndSeasonings = 11,
    [Description("Baking ingredients")] BakingIngredients = 12,
    [Description("Pasta and rice")] PastaAndRice = 13,
    [Description("Breakfast foods")] BreakfastFoods = 14,
    [Description("Baby food and formula")] BabyFoodAndFormula = 15,
    [Description("Pet food and supplies")] PetFoodAndSupplies = 16
}
public enum ItemUnit
{
    [Description("Kg")] Kilogram = 1, // for Produce, Meat and Poultry, Seafood
    [Description("g")] Gram = 2, // for Produce, Meat and Poultry, Seafood
    [Description("Dozen")] Dozen = 3, // for Dairy and Eggs
    [Description("Loaf")] Loaf = 4, // for Bakery Items
    [Description("Can")] Can = 5, // for Canned and Jarred Goods
    [Description("Packet")] Packet = 6, // for Frozen Foods
    [Description("Bottle")] Bottle = 7, // for Beverages
    [Description("Jar")] Jar = 8, // for Condiments and Sauces
    [Description("Container")] Container = 9, // for Baby Food and Formula, Pet Food and Supplies
    [Description("Box")] Box = 10 // for Snacks and Chips, Baking Ingredients, Pasta and Rice, Breakfast Foods
}

public enum SubscriptionState
{
    [Description("Active")] Active = 1,
    [Description("Paused")] Paused = 2,
    [Description("Cancelled")] Cancelled = 3,
    [Description("Future")] Future = 4,
    [Description("False")] False = 5,
    [Description("InTrial")] InTrial = 6,
    [Description("NonRenewing")] NonRenewing = 7,
}
public enum PantryItemFrequency
{
    [Description("Just Once")] JustOnce = 1,
    [Description("Every Box")] EveryBox = 2,
    [Description("Every Second Box")] EverySecondBox = 3,
}
public enum ShippingFrequency
{
    [Description("OneOff")] OneOff = 1,
    [Description("Weekly")] Weekly = 2,
    [Description("Fortnightly")] Fortnightly = 3,
}
public enum DeliveryScheduleType
{
    [Description("Post Code Delivery")] PostCodeDelivery = 1,
    [Description("School Delivery")] SchoolDelivery = 2,
}
