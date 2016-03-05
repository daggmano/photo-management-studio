using System;
using Microsoft.Framework.Caching.Memory;

namespace PhotoLibraryImageService.Services
{
	public class Caching
	{
		private static Caching _instance = null;
		private readonly MemoryCache _memoryCache;

		public static Caching Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new Caching();
				}
				return _instance;
			}
		}

		private Caching()
		{
			_memoryCache = new MemoryCache(new MemoryCacheOptions() {
				CompactOnMemoryPressure = true
			});
		}

		public void AddToCache<T>(string cacheKeyName, T cacheItem) where T : class
		{
			// Add inside cache
			_memoryCache.Set(cacheKeyName, cacheItem, new MemoryCacheEntryOptions().SetSlidingExpiration(new TimeSpan(0, 10, 0)));
		}

		public T GetCachedItem<T>(string cacheKeyName) where T : class
		{
			T result;
			if (_memoryCache.TryGetValue(cacheKeyName, out result))
			{
				return result as T;
			}
			return default(T);
		}

		public void RemoveCachedItem(string cacheKeyName)
		{
			_memoryCache.Remove(cacheKeyName);
		}
	}
}
