namespace FrostAura.Libraries.Semantic.Core.Extensions.String
{
	public static class ChunkingExtensions
	{
		/// <summary>
		/// Break up a string into multiple chunks by the first encounter of <delimiter>, on or after <charCount>.
		///
		/// Example: Break up a string into chunks. Each chunk should be at least 1000 <charCount>,
		/// so each chunk would end with the first occurance of <delimiter>, after <charCount> characters.
		/// </summary>
		/// <param name="input">The input text to split into chunks.</param>
		/// <param name="charsCount">The cound of characters for each chunk to start looking for the <delimiter>.</param>
		/// <param name="delimiter">The character to look for as the delimiter to split the string into chunks.</param>
		/// <returns>A collection of string chunks.</returns>
		public static List<string> ChunkByDelimiterAfterCharsCount(this string input, int charsCount = 1000, char delimiter = '.')
		{
			var delimiterCount = input.Count(c => c == delimiter);

            if (delimiterCount <= 1) return new List<string> { input };
            if (input.Length <= charsCount) return new List<string> { input };

			var response = new List<string>();

            // START OF YOUR CODE

            // Initialize the start index for substring extraction.
            var startIndex = 0;

            while (startIndex < input.Length)
            {
                // Find the index of the first occurrence of <delimiter> after <charCount> characters.
                var delimiterIndex = input.IndexOf(delimiter, Math.Min(startIndex + charsCount, input.Length));

                // If <delimiter> is not found or it's found before reaching <charCount>, break the loop.
                if (delimiterIndex == -1 || delimiterIndex < startIndex + charsCount)
                {
                    break;
                }

                // Calculate the length of the substring to extract.
                var length = delimiterIndex - startIndex + 1;

                // Extract the chunk from the input string and add it to the response.
                var chunk = input.Substring(startIndex, length);
                response.Add(chunk);

                // Update the start index for the next iteration.
                startIndex = delimiterIndex + 1;
            }

            // If there are remaining characters, add them as the last chunk.
            if (startIndex < input.Length)
            {
                var remainingChunk = input.Substring(startIndex);

                response.Add(remainingChunk);
            }

            // END OF YOUR CODE

            return response;
		}
	}
}
