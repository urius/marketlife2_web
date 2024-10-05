using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Tools.StreamingAudioPlayer
{
    public class StreamingAudioLoader : MonoBehaviour, IStreamingAudioLoader
    {
        private AudioClip _currentStreamingAudioClip;

        public UniTask<AudioClip> GetStreamingAudioClip(string streamURL, AudioType audioType, ulong bytesBeforePlayback)
        {
            var tcs = new UniTaskCompletionSource<AudioClip>();
            
            StartCoroutine(LoadAudioAsyncCoroutine(tcs, streamURL, audioType, bytesBeforePlayback));

            return tcs.Task;
        }

        private static IEnumerator LoadAudioAsyncCoroutine(
            IPromise<AudioClip> taskCompletionSource, string streamURL, AudioType audioType, ulong bytesBeforePlayback)
        {
            if (string.IsNullOrWhiteSpace(streamURL))
            {
                Debug.LogWarning("No audio stream URL provided. Playback skipped.");
                yield break;
            }

            using var webRequest = UnityWebRequestMultimedia.GetAudioClip(streamURL, audioType);

            var audioDownloadHandler = (DownloadHandlerAudioClip)webRequest.downloadHandler;
            audioDownloadHandler.streamAudio = true;
            var download = webRequest.SendWebRequest();

            yield return null;

            while (true)
            {
                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    var errorMsg = $"Error downloading audio stream from:{streamURL} : {webRequest.error}";
                    Debug.LogWarning(errorMsg);
                    taskCompletionSource.TrySetException(new Exception(errorMsg));

                    yield break;
                }

                if (download.webRequest.downloadedBytes >= bytesBeforePlayback)
                {
                    break;
                }

                yield return new WaitForSecondsRealtime(0.2f);
            }

            var audioClip = audioDownloadHandler.audioClip;

            if (audioClip == null)
            {
                const string errorMsg = "Couldn't process audio stream";
                Debug.LogWarning(errorMsg);
                taskCompletionSource.TrySetException(new Exception(errorMsg));
                
                yield break;
            }

            taskCompletionSource.TrySetResult(audioClip);
            Debug.Log("Playing audio stream!");
            
            yield return download;

            Debug.Log($"Finished downloading audio stream! TotalBytes: {webRequest.downloadedBytes}");
        }
    }

    public interface IStreamingAudioLoader
    {
        public UniTask<AudioClip> GetStreamingAudioClip(string streamURL, AudioType audioType = AudioType.UNKNOWN,
            ulong bytesBeforePlayback = 300000);
    }
}