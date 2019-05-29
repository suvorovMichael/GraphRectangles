using UnityEngine;
using UnityEngine.EventSystems;

public class Edge : MonoBehaviour, IPointerClickHandler {

    RectTransform rectTransform;

    public void Initialize(Node first, Node second) {
        name = "Edge";

        rectTransform = gameObject.GetComponent<RectTransform>();
        transform.localScale = Vector2.one;
        NodeFirst = first;
        NodeSecond = second;

        UpdateTransform();

        first.AddEdge(this);
        second.AddEdge(this);
    }

    public Node NodeFirst { get; private set; }
    public Node NodeSecond { get; private set; }

    public void UpdateTransform() {
        rectTransform.localPosition = NodeFirst.transform.localPosition + (NodeSecond.transform.localPosition - NodeFirst.transform.localPosition) / 2;
        rectTransform.sizeDelta = new Vector2(Vector2.Distance(NodeSecond.transform.localPosition, NodeFirst.transform.localPosition), rectTransform.sizeDelta.y);
        rectTransform.localRotation = Quaternion.Euler(0, 0, -Vector2.SignedAngle(NodeFirst.transform.localPosition - NodeSecond.transform.localPosition, Vector2.left));
    }

    public void Delete() {
        NodeFirst.RemoveEdge(this);
        NodeSecond.RemoveEdge(this);
        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData) {
        Delete();
    }
}
