using System.ComponentModel;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Models.Thoughts;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using FrostAura.Libraries.Semantic.Core.Thoughts.IO;
using FrostAura.Libraries.Semantic.Core.Thoughts.Media;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive
{
	public class YouTubeShortFactualVideoGenerationChain : BaseExecutableChain
	{
        /// <summary>
        /// An example query that this chain example can be used to solve for.
        /// </summary>
        public override string QueryExample => "Create a short factual video in documentary style on any interesting cosmos/space topic and push it to YouTube.";
        /// <summary>
        /// An example query input that this chain example can be used to solve for.
        /// </summary>
        public override string QueryInputExample => "cosmos/space";
        /// The reasoning for the solution of the chain.
        /// </summary>
        public override string Reasoning => "I can chain a couple of LLM calls together in order to generate creative content, I can then download some relevant videos (I think 6 should be enough) and edit it together creatively using code interpreter, since I know Python well.";
        /// <summary>
        /// A collection of thoughts.
        /// </summary>
        public override List<Thought> ChainOfThoughts => new List<Thought>
        {
            #region Generate Title, Description & Tags
            new Thought
            {
                Action = $"{nameof(LanguageModelThoughts)}.{nameof(LanguageModelThoughts.PromptLLMAsync)}",
                Reasoning = "I will generate a captivating title that's likely to go viral on a platform like YouTube.",
                Critisism = "I should be careful not to return the same response each time, given the same topic so will keep my prompt general.",
                Arguments = new Dictionary<string, string>
                {
                    { "prompt", """
                        On the theme of "$input", think about at least 10 exciting, random, but factual topics that could make for a viral YouTube short video documentary and that you would have the knowledge to write a script about.
                        Then pick a random one and give me a title for the topic that you've chosen, that I can use for the YouTube video.
                        Do not list any of your considerations.
                        Only respond with the title for your choice.

                        Title: 
                        """ }
                },
                OutputKey = "TITLE"
            },
            new Thought
            {
                Action = $"{nameof(LanguageModelThoughts)}.{nameof(LanguageModelThoughts.PromptLLMAsync)}",
                Reasoning = "I will generate a factual but captivating script for the video that is short enough for a YouTube short video.",
                Critisism = "I should be sure to stick to the facts when writing the script. I should also be as specific in my prompt to only get exactly what I want, the script.",
                Arguments = new Dictionary<string, string>
                {
                    { "prompt", """
                        Write an essay of 100 words on interesting facts for a documentary-style video with the title "$TITLE".
                        Make it interesting and factual. Don't return it in point form rather make it story-like.
                        Do not list any of your considerations.
                        Do not include the title in the essay.
                        Only respond with the body of the essay for your choice (don't include the title etc).

                        Essay Body: 
                        """ }
                },
                OutputKey = "SCRIPT"
            },
            new Thought
            {
                Action = $"{nameof(LanguageModelThoughts)}.{nameof(LanguageModelThoughts.PromptLLMAsync)}",
                Reasoning = "I will generate a short description for the YouTube video based on the script.",
                Critisism = "I should keep it captivating since this is for a social platform.",
                Arguments = new Dictionary<string, string>
                {
                    { "prompt", """
                        Write a captivating short description for the script video with the following script.

                        Script: $SCRIPT

                        Description: 
                        """ }
                },
                OutputKey = "DESCRIPTION"
            },
            new Thought
            {
                Action = $"{nameof(LanguageModelThoughts)}.{nameof(LanguageModelThoughts.PromptLLMAsync)}",
                Reasoning = "I will generate a space-delimited list of tags for the YouTube video that should help it go viral.",
                Critisism = "I should have a couple of good tags but not over-do it.",
                Arguments = new Dictionary<string, string>
                {
                    { "prompt", """
                        Give me up to 10 tags for a YouTube short video, based on the following script.
                        Each tag should be lower-case only.
                        Each tag, if it consists of more than a single word, should be joined by -. For example, tag-with-more-words
                        Give the tags to me as a space-delimited string. For example, Tags: tag-1 tag-2 tag-3

                        Script: $SCRIPT

                        Tags: 
                        """ }
                },
                OutputKey = "TAGS"
            },
            #endregion

            #region Search for & Download Videos
            new Thought
            {
                Action = $"{nameof(LanguageModelThoughts)}.{nameof(LanguageModelThoughts.PromptLLMAsync)}",
                Reasoning = "I will generate a search query for a stock video which I can later use to download a stock video. This is just for video 1 of 6.",
                Critisism = "Doing this for each video is inefficient.",
                Arguments = new Dictionary<string, string>
                {
                    { "prompt", """
                        Given a YouTube short video script, generate a SINGLE (no comma-separated queries), short and consise search query I could use on a stock video website like Pexels to find relevant content.
                        ---
                        Script: $SCRIPT
                        ---
                        Search Query: 
                        """ }
                },
                OutputKey = "VIDEO_QUERY_1"
            },
            new Thought
            {
                Action = $"{nameof(LanguageModelThoughts)}.{nameof(LanguageModelThoughts.PromptLLMAsync)}",
                Reasoning = "I will generate a search query for a stock video which I can later use to download a stock video. This is just for video 2 of 6.",
                Critisism = "Doing this for each video is inefficient.",
                Arguments = new Dictionary<string, string>
                {
                    { "prompt", """
                        Given a YouTube short video script, generate a SINGLE (no comma-separated queries), short and consise search query I could use on a stock video website like Pexels to find relevant content.
                        Also DO NOT use the following items as they are already used. Instead produce a search query that's diverse from the items listed below.
                        - $VIDEO_QUERY_1
                        ---
                        Script: $SCRIPT
                        ---
                        Search Query: 
                        """ }
                },
                OutputKey = "VIDEO_QUERY_2"
            },
            new Thought
            {
                Action = $"{nameof(LanguageModelThoughts)}.{nameof(LanguageModelThoughts.PromptLLMAsync)}",
                Reasoning = "I will generate a search query for a stock video which I can later use to download a stock video. This is just for video 3 of 6.",
                Critisism = "Doing this for each video is inefficient.",
                Arguments = new Dictionary<string, string>
                {
                    { "prompt", """
                        Given a YouTube short video script, generate a SINGLE (no comma-separated queries), short and consise search query I could use on a stock video website like Pexels to find relevant content.
                        Also DO NOT use the following items as they are already used. Instead produce a search query that's diverse from the items listed below.
                        - $VIDEO_QUERY_1
                        - $VIDEO_QUERY_2
                        ---
                        Script: $SCRIPT
                        ---
                        Search Query: 
                        """ }
                },
                OutputKey = "VIDEO_QUERY_3"
            },
            new Thought
            {
                Action = $"{nameof(LanguageModelThoughts)}.{nameof(LanguageModelThoughts.PromptLLMAsync)}",
                Reasoning = "I will generate a search query for a stock video which I can later use to download a stock video. This is just for video 4 of 6.",
                Critisism = "Doing this for each video is inefficient.",
                Arguments = new Dictionary<string, string>
                {
                    { "prompt", """
                        Given a YouTube short video script, generate a SINGLE (no comma-separated queries), short and consise search query I could use on a stock video website like Pexels to find relevant content.
                        Also DO NOT use the following items as they are already used. Instead produce a search query that's diverse from the items listed below.
                        - $VIDEO_QUERY_1
                        - $VIDEO_QUERY_2
                        - $VIDEO_QUERY_3
                        ---
                        Script: $SCRIPT
                        ---
                        Search Query: 
                        """ }
                },
                OutputKey = "VIDEO_QUERY_4"
            },
            new Thought
            {
                Action = $"{nameof(LanguageModelThoughts)}.{nameof(LanguageModelThoughts.PromptLLMAsync)}",
                Reasoning = "I will generate a search query for a stock video which I can later use to download a stock video. This is just for video 5 of 6.",
                Critisism = "Doing this for each video is inefficient.",
                Arguments = new Dictionary<string, string>
                {
                    { "prompt", """
                        Given a YouTube short video script, generate a SINGLE (no comma-separated queries), short and consise search query I could use on a stock video website like Pexels to find relevant content.
                        Also DO NOT use the following items as they are already used. Instead produce a search query that's diverse from the items listed below.
                        - $VIDEO_QUERY_1
                        - $VIDEO_QUERY_2
                        - $VIDEO_QUERY_3
                        - $VIDEO_QUERY_4
                        ---
                        Script: $SCRIPT
                        ---
                        Search Query: 
                        """ }
                },
                OutputKey = "VIDEO_QUERY_5"
            },
            new Thought
            {
                Action = $"{nameof(LanguageModelThoughts)}.{nameof(LanguageModelThoughts.PromptLLMAsync)}",
                Reasoning = "I will generate a search query for a stock video which I can later use to download a stock video. This is just for video 6 of 6.",
                Critisism = "Doing this for each video is inefficient.",
                Arguments = new Dictionary<string, string>
                {
                    { "prompt", """
                        Given a YouTube short video script, generate a SINGLE (no comma-separated queries), short and consise search query I could use on a stock video website like Pexels to find relevant content.
                        Also DO NOT use the following items as they are already used. Instead produce a search query that's diverse from the items listed below.
                        - $VIDEO_QUERY_1
                        - $VIDEO_QUERY_2
                        - $VIDEO_QUERY_3
                        - $VIDEO_QUERY_4
                        - $VIDEO_QUERY_5
                        ---
                        Script: $SCRIPT
                        ---
                        Search Query: 
                        """ }
                },
                OutputKey = "VIDEO_QUERY_6"
            },
            new Thought
            {
                Action = $"{nameof(StockMediaThoughts)}.{nameof(StockMediaThoughts.DownloadAndGetStockVideoAsync)}",
                Reasoning = "I will download a stock video for each query generated. This is for query 1 of 6.",
                Critisism = "Doing this for each video is inefficient. Would be good to make this concurrent somehow.",
                Arguments = new Dictionary<string, string>
                {
                    { "searchQuery", "$VIDEO_QUERY_1" },
                    { "orientation", "portrait" }
                },
                OutputKey = "VIDEO_1_PATH"
            },
            new Thought
            {
                Action = $"{nameof(StockMediaThoughts)}.{nameof(StockMediaThoughts.DownloadAndGetStockVideoAsync)}",
                Reasoning = "I will download a stock video for each query generated. This is for query 2 of 6.",
                Critisism = "Doing this for each video is inefficient. Would be good to make this concurrent somehow.",
                Arguments = new Dictionary<string, string>
                {
                    { "searchQuery", "$VIDEO_QUERY_2" },
                    { "orientation", "portrait" }
                },
                OutputKey = "VIDEO_2_PATH"
            },
            new Thought
            {
                Action = $"{nameof(StockMediaThoughts)}.{nameof(StockMediaThoughts.DownloadAndGetStockVideoAsync)}",
                Reasoning = "I will download a stock video for each query generated. This is for query 3 of 6.",
                Critisism = "Doing this for each video is inefficient. Would be good to make this concurrent somehow.",
                Arguments = new Dictionary<string, string>
                {
                    { "searchQuery", "$VIDEO_QUERY_3" },
                    { "orientation", "portrait" }
                },
                OutputKey = "VIDEO_3_PATH"
            },
            new Thought
            {
                Action = $"{nameof(StockMediaThoughts)}.{nameof(StockMediaThoughts.DownloadAndGetStockVideoAsync)}",
                Reasoning = "I will download a stock video for each query generated. This is for query 4 of 6.",
                Critisism = "Doing this for each video is inefficient. Would be good to make this concurrent somehow.",
                Arguments = new Dictionary<string, string>
                {
                    { "searchQuery", "$VIDEO_QUERY_4" },
                    { "orientation", "portrait" }
                },
                OutputKey = "VIDEO_4_PATH"
            },
            new Thought
            {
                Action = $"{nameof(StockMediaThoughts)}.{nameof(StockMediaThoughts.DownloadAndGetStockVideoAsync)}",
                Reasoning = "I will download a stock video for each query generated. This is for query 5 of 6.",
                Critisism = "Doing this for each video is inefficient. Would be good to make this concurrent somehow.",
                Arguments = new Dictionary<string, string>
                {
                    { "searchQuery", "$VIDEO_QUERY_5" },
                    { "orientation", "portrait" }
                },
                OutputKey = "VIDEO_5_PATH"
            },
            new Thought
            {
                Action = $"{nameof(StockMediaThoughts)}.{nameof(StockMediaThoughts.DownloadAndGetStockVideoAsync)}",
                Reasoning = "I will download a stock video for each query generated. This is for query 6 of 6.",
                Critisism = "Doing this for each video is inefficient. Would be good to make this concurrent somehow.",
                Arguments = new Dictionary<string, string>
                {
                    { "searchQuery", "$VIDEO_QUERY_6" },
                    { "orientation", "portrait" }
                },
                OutputKey = "VIDEO_6_PATH"
            },
            #endregion

            #region Synthesize Voice-over
            new Thought
            {
                Action = $"{nameof(AudioThoughts)}.{nameof(AudioThoughts.TextToSpeechAsync)}",
                Reasoning = "I will generate a voice-over for the video that speaks the script fashionably.",
                Critisism = "I don't have control over the pitch and speed of the voice here.",
                Arguments = new Dictionary<string, string>
                {
                    { "text", "$SCRIPT" }
                },
                OutputKey = "VOICEOVER_AUDIO_PATH"
            },
            #endregion

            #region Edit Video via Code Interpreter
            new Thought
            {
                Action = $"{nameof(CodeInterpreterThoughts)}.{nameof(CodeInterpreterThoughts.InvokeAsync)}",
                Reasoning = "I will use my code Python code interpreter to construct a script that can use the MoviePy library to put a creative video together with all the content we now have and respond with the video file's path.",
                Critisism = "I need to ensure that I use the correct package versions so that the Python environment has the required dependencies installed otherwise my Python code may not execute.",
                Arguments = new Dictionary<string, string>
                {
                    { "pythonVersion", "3.8" },
                    { "pipDependencies", "pydub==0.25.1 moviepy==1.0.3 Pillow==9.5.0" },
                    { "condaDependencies", "ffmpeg imagemagick" },
                    { "code", """
                        def main() -> str:
                            # Set up constants and I use tripple quotes to make sure any strings are safe when interpolated.
                            video_transcript = '''$SCRIPT'''
                            voice_over_audio_track_path = '''$VOICEOVER_AUDIO_PATH'''
                            video_paths = [
                                '''$VIDEO_1_PATH''',
                                '''$VIDEO_2_PATH''',
                                '''$VIDEO_3_PATH''',
                                '''$VIDEO_4_PATH''',
                                '''$VIDEO_5_PATH''',
                                '''$VIDEO_6_PATH'''
                            ]

                            import moviepy.editor as mp
                            from moviepy.editor import VideoFileClip, CompositeVideoClip, TextClip, ColorClip
                            from moviepy.audio.io.AudioFileClip import AudioFileClip
                            import random
                            import uuid

                            def split_video_into_subclips(video_path, max_subclip_duration, target_resolution):
                                video_clip = VideoFileClip(video_path)
                                total_duration = video_clip.duration

                                start_times = range(0, int(total_duration), max_subclip_duration)
                                subclips = []

                                for start_time in start_times:
                                    end_time = min(start_time + max_subclip_duration, total_duration)
                                    subclip = video_clip.subclip(start_time, end_time)
                                    subclip = subclip.resize(target_resolution)

                                    subclips.append(subclip)

                                return subclips

                            def get_caption_sentences_with_durations(transcript, audio_duration):
                                sentences = [f'{l}.' for l in transcript.split('.') if len(l) > 0]
                                total_sentence_duration = sum(len(sentence.split()) for sentence in sentences)
                                caption_durations = [(len(sentence.split()) / total_sentence_duration) * audio_duration for sentence in sentences]
                                result = []

                                for idx, ele in enumerate(sentences):
                                    result.append({
                                        'text': sentences[idx],
                                        'duration': caption_durations[idx]
                                    })

                                return result

                            def get_captions_clip(sentences, target_resolution, margin = 500):
                                # Create a list to store the sentence video clips.
                                sentence_clips = []
                                # We need a placeholder clip just to make the text scale and position properly when later overlayed onto the main video.
                                blank_clip = ColorClip(size=target_resolution, color=(0, 0, 0, 0)).set_duration(1)

                                # Create each sentence video clip and add it to the list.
                                for sentence in sentences:
                                    sentence_clip = TextClip(
                                        sentence['text'],
                                        method='caption',
                                        align='center',
                                        interline=0,
                                        fontsize=55,
                                        size=target_resolution,
                                        color='white')
                                    sentence_clip = sentence_clip.set_duration(sentence['duration'])
                                    sentence_clip = CompositeVideoClip([
                                        blank_clip,
                                        sentence_clip.set_position(('center', target_resolution[1] - sentence_clip.size[1] + margin))
                                    ], size=target_resolution)

                                    sentence_clips.append(sentence_clip)

                                # Combine all sentence video clips into a single video.
                                return mp.concatenate_videoclips(sentence_clips, method='compose') 

                            target_resolution = (1080, 1920)
                            video_name = str(uuid.uuid4()).replace('-', '')
                            output_path = f'videos/{video_name}.mp4'
                            # Get windowed, randomized clips from all the videos.
                            clips = [split_video_into_subclips(vp, 10, target_resolution) for vp in video_paths]
                            clips = [subclip for sublist in clips for subclip in sublist]
                            shuffled_windowed_clips = random.sample(clips, len(clips))

                            # Concat all random video clips together.
                            shuffled_windowed_clip = mp.concatenate_videoclips(shuffled_windowed_clips, method='compose')

                            # Set the audio track to the voice over and make the video's length that of the voice over track length.
                            voice_over_audio_clip = AudioFileClip(voice_over_audio_track_path)
                            final_video = shuffled_windowed_clip.subclip(0, voice_over_audio_clip.duration)
                            final_video = final_video.set_audio(voice_over_audio_clip)

                            # Combine all sentence video clips into a single video.
                            sentences = get_caption_sentences_with_durations(video_transcript, voice_over_audio_clip.duration)
                            sentences_video = get_captions_clip(sentences, target_resolution)

                            # Overlay layer to make the text pop.
                            black_layer = ColorClip(size=target_resolution, color=(0, 0, 0))
                            black_layer = black_layer.set_duration(final_video.duration)
                            black_layer = black_layer.set_opacity(0.30)

                            # Combine all sentence video clips into a single video.
                            final_video = CompositeVideoClip([final_video, black_layer, sentences_video])

                            print(f'Generating output video to path: {output_path}')
                            # Turning off any kind of logging or progress bars as it may interfere with the code interpreter.
                            final_video.write_videofile(output_path, codec='h264', audio_codec='aac', verbose=False, logger=None)

                            return output_path
                        """ }
                },
                OutputKey = "VIDEO_PATH"
            },
            #endregion

            new Thought
            {
                Action = $"{nameof(YouTubeThoughts)}.{nameof(YouTubeThoughts.PublishLocalVideoToYouTubeAsync)}",
                Reasoning = "I can use the YouTube publish thought to upload the YouTube short video.",
                Arguments = new Dictionary<string, string>
                {
                    { "filePath", "$VIDEO_PATH" },
                    { "description", "$DESCRIPTION" },
                    { "tags", "$TAGS" }
                },
                OutputKey = "YOUTUBE_OUT"
            },

            new Thought
            {
                Action = $"{nameof(OutputThoughts)}.{nameof(OutputThoughts.OutputTextAsync)}",
                Reasoning = "I can simply proxy the response as a direct and response is appropriate for an exact transcription.",
                Arguments = new Dictionary<string, string>
                {
                    { "output", "A short video called '$TITLE' has been created and pushed to YouTube." }
                },
                OutputKey = "OUT"
            }
        };

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="serviceProvider">The dependency service provider.</param>
        /// <param name="logger">Instance logger.</param>
        public YouTubeShortFactualVideoGenerationChain(IServiceProvider serviceProvider, ILogger<YouTubeShortFactualVideoGenerationChain> logger)
            : base(serviceProvider, logger)
        { }

        /// <summary>
        /// Execute the chain of thought sequentially.
        /// </summary>
        /// <param name="input">The initial input into the chain.</param>
        /// <param name="state">The optional state of the chain. Should state be provided for outputs, thoughts that produce such outputs would be skipped.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>The chain's final output.</returns>
        public override Task<string> ExecuteChainAsync(string input = "", Dictionary<string, string> state = null, CancellationToken token = default)
        {
            return base.ExecuteChainAsync(input.ThrowIfNullOrWhitespace(nameof(input)), state, token);
        }

        /// <summary>
        /// Generate a factual documentary style video on a provided topic and return the video file's local file path.
        /// </summary>
        /// <param name="topic">The topic that the video should be on.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>The video file's local file path.</returns>
        [KernelFunction, Description("Generate a factual documentary style video on a provided topic and return the video file's local file path.")]
        public Task<string> TranscribeAudioFileAsync(
            [Description("The topic that the video should be on.")] string topic,
            CancellationToken token = default)
        {
            return ExecuteChainAsync(topic, token: token);
        }
    }
}
