using FrostAura.Libraries.Semantic.Core.Extensions.String;
using Xunit;

namespace FrostAura.Libraries.Semantic.Core.Tests.Extensions.String;

	public class ChunkingExtensionsTests
	{
		[Fact]
		public void ChunkByDelimiterAfterCharsCount_WithInputLessThanCharCount_ShouldReturnSingleChunk()
		{
        var charCount = 1000;
        var delimiter = '.';
        var input = "Hello World!";
			var expected = 1;

			var actual = input.ChunkByDelimiterAfterCharsCount(charCount, delimiter);

			Assert.Equal(expected, actual.Count);
    }

		[Fact]
		public void ChunkByDelimiterAfterCharsCount_WitSingleDelimiter_ShouldReturnSingleChunk()
		{
			var charCount = 1000;
			var delimiter = '.';
        var input = new string(Enumerable
				.Range(0, charCount)
				.Select(i => 'a')
				.ToArray()) + delimiter;
        var expected = 1;

        var actual = input.ChunkByDelimiterAfterCharsCount(charCount, delimiter);

        Assert.Equal(expected, actual.Count);
    }

    [Fact]
    public void ChunkByDelimiterAfterCharsCount_WithNoDelimiterInInput_ShouldReturnSingleChunk()
    {
        var charCount = 1000;
        var delimiter = '.';
        var input = new string(Enumerable
            .Range(0, charCount * 2)
            .Select(i => 'a')
            .ToArray());
        var expected = 1;

        var actual = input.ChunkByDelimiterAfterCharsCount(charCount, delimiter);

        Assert.Equal(expected, actual.Count);
    }

    [Fact]
		public void ChunkByDelimiterAfterCharsCount_WithMultipleTextSegmentsSmallerThanCharCount_ShouldReturnSingleChunk()
		{
        var charCount = 1000;
        var delimiter = '.';

			var input1 = new string(Enumerable
            .Range(0, charCount/4)
            .Select(i => 'a')
            .ToArray()) + delimiter;
        var input2 = new string(Enumerable
            .Range(0, charCount / 4)
            .Select(i => 'b')
            .ToArray()) + delimiter;
        var input3 = new string(Enumerable
            .Range(0, charCount / 4)
            .Select(i => 'c')
            .ToArray()) + delimiter;

        var input = $"{input1} {input2} {input3}";
        var expected = 1;

        var actual = input.ChunkByDelimiterAfterCharsCount(charCount, delimiter);

        Assert.Equal(expected, actual.Count);
    }

		[Fact]
		public void ChunkByDelimiterAfterCharsCount_WithTwoLargeTextSegments_ShouldReturnTwoChunks()
		{
        var charCount = 1000;
        var delimiter = '.';
        var input1 = new string(Enumerable
            .Range(0, charCount * 2)
            .Select(i => 'a')
            .ToArray()) + delimiter;
        var input2 = new string(Enumerable
            .Range(0, charCount * 3)
            .Select(i => 'b')
            .ToArray()) + delimiter;
        var input = $"{input1} {input2}";
        var expected = 2;

        var actual = input.ChunkByDelimiterAfterCharsCount(charCount, delimiter);

        Assert.Equal(expected, actual.Count);
    }

    [Fact]
		public void ChunkByDelimiterAfterCharsCount_WithTwoLargeAndOneSmallTextSegment_ShouldReturnThreeChunks()
		{
        var charCount = 1000;
        var delimiter = '.';
        var input1 = new string(Enumerable
            .Range(0, charCount * 2)
            .Select(i => 'a')
            .ToArray()) + delimiter;
        var input2 = new string(Enumerable
            .Range(0, charCount * 3)
            .Select(i => 'b')
            .ToArray()) + delimiter;
        var input3 = new string(Enumerable
            .Range(0, charCount / 2)
            .Select(i => 'c')
            .ToArray()) + delimiter;
        var input = $"{input1} {input2} {input3}";
        var expected = 3;

        var actual = input.ChunkByDelimiterAfterCharsCount(charCount, delimiter);

        Assert.Equal(expected, actual.Count);
    }
}
