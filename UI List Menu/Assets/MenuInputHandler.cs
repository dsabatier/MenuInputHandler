using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MenuInputHandler : MonoBehaviour {

    public float repeatSpeed = 0.5f;
    public float scrollingRepeatSpeed = 0.1f;
    public float scrollingRepeatDelay = 3f;

    private List<GameObject> buttons;
    private int selectedButtonIndex;
    private EventSystem es;

    private float holdTimer;
    private float deadzone = 0.2f;
    private bool canScroll = true;

	void Start () {

        CreateNewButtonList();
        InitializeMenu();
  
	}

    void Update()
    {
        // this makes sure that our menu is always going to have focus
        if (!es.currentSelectedGameObject) es.SetSelectedGameObject(buttons[selectedButtonIndex]);

        float input = Input.GetAxis("Vertical");
        HandleDirectionalInput(input);

        if (Input.GetButtonDown("Submit"))
        {
            ClickSelectedButton();
        }

    }

    // creates new list of buttons from any child gameObject with a Button component
    private void CreateNewButtonList()
    {
        buttons = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.GetComponent<Button>())
                buttons.Add(child.gameObject);
        }
    }

    private void InitializeMenu()
    {
        if (!es) es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        if (buttons.Count > 0) SelectButton(0);
    }

    private void HandleDirectionalInput(float input)
    {
        // if input magnitude exceeds our deadzone select next item, otherwise reset our timers and checks
        if (Mathf.Abs(input) > deadzone)
        {
            holdTimer += Time.deltaTime;

            if (Mathf.Sign(input) > 0)          // up input  is entered
            {
                if (canScroll) SelectPreviousButton();

            }
            
            else if (Mathf.Sign(input) < 0)     // down input is entered
            {
                if (canScroll) SelectNextButton();
            }
        }
        else if (Mathf.Abs(input) < deadzone)
        {
            ResetTimer();  

        }
    }

    // Select a new button and scroll if the player has been providing input long enough
    private void SelectButton(int buttonIndex)
    {

        selectedButtonIndex = buttonIndex;
        es.SetSelectedGameObject(buttons[buttonIndex]);
        canScroll = false;

        if (holdTimer < scrollingRepeatDelay)
        {
            StartCoroutine("InputWaitTimer", repeatSpeed);
        }
        else
        {
            StartCoroutine("InputWaitTimer", scrollingRepeatSpeed);
        }

    }

    // next button in our list with wrap around to beginning of list if required
    private void SelectNextButton()
    {
        SelectButton((selectedButtonIndex + 1) % buttons.Count);
    }

    // previous button in our list with wrap around to end of list if required
    private void SelectPreviousButton()
    {
        int newSelection = selectedButtonIndex < 1 ? buttons.Count - 1 : selectedButtonIndex - 1;
        SelectButton(newSelection);
    }

    // resets our timer, used when player reverses input/stops providing input
    private void ResetTimer()
    {
        canScroll = true;
        holdTimer = 0;
        StopCoroutine("InputWaitTimer");
    }

    // handles the time in between scrolls
    private IEnumerator InputWaitTimer(float timer)
    {
        yield return new WaitForSeconds(timer);
        canScroll = true;
    }

    // send a message to the selected button
    private void ClickSelectedButton()
    {
        buttons[selectedButtonIndex].SendMessage("Click", SendMessageOptions.DontRequireReceiver);
        
    }
}
