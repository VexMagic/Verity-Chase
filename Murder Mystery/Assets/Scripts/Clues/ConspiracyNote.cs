using UnityEngine;

[CreateAssetMenu(fileName = "New Conspiracy Note", menuName = "Clue/Conspiracy Note")]
public class ConspiracyNote : ScriptableObject
{
    [SerializeField] private string Title;
    [SerializeField] [TextArea] private string Description;
    [SerializeField] private Sprite Sprite;

    public string title => Title;
    public string description => Description;
    public Sprite sprite => Sprite;
}
