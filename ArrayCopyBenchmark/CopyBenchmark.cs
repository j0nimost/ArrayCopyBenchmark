using BenchmarkDotNet.Attributes;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ArrayCopyBenchmark
{
    [MemoryDiagnoser]
    public class CopyBenchmark
    {
        private int[] Destination;

        [GlobalSetup]
        public void GlobalSetup()
        {
            Destination = Enumerable.Range(0, 2048).ToArray();
        }
        public IEnumerable<object> GetIntArray()
        {
            yield return Enumerable.Range(0, 10).ToArray();
            yield return Enumerable.Range(0, 1024).ToArray();
        }

        [Benchmark]
        [ArgumentsSource(nameof(GetIntArray))]
        public void ForLoopCopy(int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Destination[i] = array[i];
            }
        }

        [Benchmark]
        [ArgumentsSource(nameof(GetIntArray))]
        public void ArrayCopy(int[] array)
        {
            Array.Copy(array, Destination, array.Length);
        }

        [Benchmark]
        [ArgumentsSource(nameof(GetIntArray))]
        public void BufferBlockCopy(int[] array)
        {
            Buffer.BlockCopy(array, 0, Destination, 0, array.Length * 4);
        }

        [Benchmark]
        [ArgumentsSource(nameof(GetIntArray))]
        public void SpanCopy(int[] array)
        {
            array.AsSpan().CopyTo(Destination);
        }

        [Benchmark]
        [ArgumentsSource(nameof(GetIntArray))]
        public void MarshalCopy(int[] array)
        {
            int size = 4 * array.Length;
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(array, 0, ptr, array.Length);
            Marshal.Copy(ptr, Destination, 0, array.Length);
            Marshal.FreeHGlobal(ptr);
        }
    }
}
