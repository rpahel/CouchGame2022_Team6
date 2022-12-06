namespace Data
{
    #region CUBE_TYPE
    public enum CUBE_TYPE
    {
        NONE,
        EDIBLE,
        TNT,
        TRAP,
        BEDROCK
    }
    #endregion
    #region SKIN_SIZE
    public enum SKIN_SIZE
    {
        LITTLE,
        MEDIUM,
        BIG
    }
    #endregion
    #region LEVEL_STATE
    public enum LEVEL_STATE
    {
        NONE,
        INITIALISING,
        LOADING,
        LOADED
    }
    #endregion
    #region GAME_STATE
    public enum GAME_STATE
    {
        NONE,
        MENU,
        LOADING,
        PLAYING,
        END
    }
    #endregion
}
