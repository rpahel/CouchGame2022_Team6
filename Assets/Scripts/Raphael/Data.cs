namespace Data
{
    #region CUBE_TYPE
    public enum CUBE_TYPE
    {
        NONE,
        EDIBLE,
        TRAP,
        BEDROCK
    }
    #endregion
    #region PlayerState
    public enum PlayerState
    {
        Aiming,
        Shooting,
        Dashing,
        WallJumping,
        Moving,
    }
    #endregion
    #region PLAYER_STATE
    public enum PLAYER_STATE
    {
        WALKING,
        FALLING,
        DASHING,
        SHOOTING,
        KNOCKBACKED,
        STUNNED,
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
        LOADING,
        PLAYING,
        END
    }
    #endregion
}
