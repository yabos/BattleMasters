
public enum eMessageCategory : int
{
    System = 0x0100,
    Scene = 0x0200,
    Game = 0x0300,
    Actor = 0x0400,
    Player = 0x0500,

    Trigger = 0x0600,
    Spawner = 0x0601,

    Event = 0x0700,
    Network = 0x0800,
    UI = 0x0900,

    AI = 0x0a00,

    MouseCategoryMsg = 0x0001,
    KeyCategoryMsg = 0x0002,
}

public enum eMessage : int
{
    None = 0,

    Asset = eMessageCategory.System << 16 | 0x0001,
    UI = eMessageCategory.System << 16 | 0x0002,
    Page = eMessageCategory.System << 16 | 0x0003,

    PageTransition = eMessageCategory.Scene << 16 | 0x0001,
}