using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.IO;

namespace WebDX.Api.Tests
{
	/// <summary>
	/// DotNet IStream implementation for testing Utils
	/// </summary>
	public class TestUcomIstream : IStream 
	{
		private MemoryStream m_MemStream;

		public TestUcomIstream()
		{
			m_MemStream = new MemoryStream();
		}

		public TestUcomIstream(byte[] testbytes)
		{
			m_MemStream = new MemoryStream(testbytes);
		}

		public void Clone(out IStream clone)
		{
			clone = null;
		}

		public void Commit(int CommitFlags)
		{
		}

		public void LockRegion(long offset, long len, int flags)
		{
		}

		public void CopyTo(IStream dest, long cb, IntPtr bRead, IntPtr bWrite)
		{
		}

		public void Read(byte[] pv, int cb, IntPtr pcvRead)
		{
			int l = m_MemStream.Read(pv, 0, cb);
			if(pcvRead != IntPtr.Zero)
				Marshal.WriteInt64(pcvRead, Convert.ToInt64(l));
		}

		public void Revert()
		{
		}

		public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
		{
		}

		public void SetSize(long libNewSize)
		{
		}

		public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG sg, int flags)
		{
            System.Runtime.InteropServices.ComTypes.STATSTG stg = new System.Runtime.InteropServices.ComTypes.STATSTG();
			sg = stg;
		}

		public void UnlockRegion(long offset, long len, int flags)
		{
		}

		public void Write(byte[] pv, int cb, IntPtr pcvWritten)
		{
			m_MemStream.Write(pv, 0, cb);
			if(pcvWritten != IntPtr.Zero)
				Marshal.WriteInt64(pcvWritten, Convert.ToInt64(cb));
		}

		public byte[] GetBytes()
		{
			return m_MemStream.ToArray();
		}
	}
}
