namespace Shared
{
	public class ServerInfoResponseObject : ResponseObject<ServerDatabaseIdentifierObject>
	{
	}

    public class ServerDatabaseIdentifierObject
    {
        public string ServerId { get; set; }
        public string ServerName { get; set; }
    }
}
