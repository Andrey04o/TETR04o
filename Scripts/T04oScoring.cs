using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using TMPro;
namespace TETR04o {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class T04oScoring : UdonSharpBehaviour
    {
        public TextMeshPro[] textMeshScores;
        public TextMeshPro[] textMeshBestScores;
        [UdonSynced] public int indexScore = 0;
        [UdonSynced] public int indexScoreMax = 0;
        public int[] linePoints = {0, 100, 300, 500, 800};
        public void AddSoftDrop() {
            indexScore += 1;
            SetScore();
        }
        public void AddHardDrop(int cells) {
            indexScore += cells * 2;
            SetScore();
        }
        public void AddPoints(int countLines, int level) {
            indexScore += linePoints[countLines] * level;
            SetScore();
        }
        public void SetScore(int score) {
            indexScore = 0;
            SetScore();
        }
        void SetScore() {
            if (indexScore > indexScoreMax) {
                indexScoreMax = indexScore;
            }
            SetText();
            RequestSerialization();
        }

        public void SetText() {
            foreach (TextMeshPro textMeshScore in textMeshScores) {
                textMeshScore.text = indexScore + "";
            }
            foreach (TextMeshPro textMeshBestScore in textMeshBestScores) {
                textMeshBestScore.text = indexScoreMax + "";
            }
        }

        public override void OnDeserialization()
        {
            base.OnDeserialization();
            SetText();
        }
    }
}