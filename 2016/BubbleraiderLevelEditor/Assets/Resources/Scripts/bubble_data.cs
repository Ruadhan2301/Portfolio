using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class bubble_data : MonoBehaviour {

    public string type = "-";
    private GameObject ball;

    public void setType(string type) {
        this.type = type;
        
        updateGraphic();
    }
    public string getType() {
        
        return type;
    }

    private void updateGraphic() {
        if (ball == null) {
            ball = transform.gameObject;
        }
        if (type == "-") {
            type = "base";
        }
        Sprite spr = Resources.Load<Sprite>("Textures/bubble_" + type + "");
        if(type == "base") {
            type = "-";
        }
        ball.GetComponent<Image>().sprite = spr;
    }

}
