using System.Runtime.InteropServices;

namespace BulkSync
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BulkBatch
	{
		public byte Id;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = BulkBatchSync.BatchSize)]
		public float[] Position;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = BulkBatchSync.BatchSize)]
		public float[] Rotation;
	}

	public static class BulkUtilities
	{
		public static byte[] Serialize(this BulkBatch batch)
		{
			var size = Marshal.SizeOf(batch);
			var arr = new byte[size];
			var ptr = Marshal.AllocHGlobal(size);
			Marshal.StructureToPtr(batch, ptr, true);
			Marshal.Copy(ptr, arr, 0, size);
			Marshal.FreeHGlobal(ptr);

			return arr;
		}

		public static BulkBatch Deserialize(byte[] data)
		{
			var batch = new BulkBatch();
			var size = Marshal.SizeOf(batch);
			var ptr = Marshal.AllocHGlobal(size);
			Marshal.Copy(data, 0, ptr, size);

			batch = (BulkBatch) Marshal.PtrToStructure(ptr, batch.GetType());
			Marshal.FreeHGlobal(ptr);

			return batch;
		}
	}
}