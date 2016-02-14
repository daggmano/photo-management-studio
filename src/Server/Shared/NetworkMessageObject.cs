namespace Shared
{
	public enum NetworkMessageType
	{
		ServerSpecification    
	}

	public interface INetworkMessageObject
	{
		NetworkMessageType MessageType { get; }
	}

	public class NetworkMessageObject : INetworkMessageObject
	{
		public NetworkMessageType MessageType { get; set; }
	}

	public class NetworkMessageObject<T> : INetworkMessageObject
	{
		public NetworkMessageType MessageType { get; set; }
		public T Message { get; set; }
	}
}
