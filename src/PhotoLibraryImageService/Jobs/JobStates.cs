namespace PhotoLibraryImageService.Jobs
{
	public enum JobStates
	{
		Unknown,
		Submitted,
		Running,
		Error,
		Complete,
		Tombstoned
	}
}
