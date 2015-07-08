namespace ProcessHelpers
{
    public enum WmiReturnValue
    {
        SuccessfullCompletion = 0,
        AccessDenied = 2,
        InsufficientPrivilege = 3,
        UnknownFailure = 8,
        PathNotFound = 9,
        InvalidParameter = 21
    }
}
