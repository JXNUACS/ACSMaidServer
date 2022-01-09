public class PermissionsDenied: ApplicationException
{
    public PermissionsDenied(string message): base(message) {}
}

public class CommonException: ApplicationException
{
    public CommonException(string message): base(message) {}
}