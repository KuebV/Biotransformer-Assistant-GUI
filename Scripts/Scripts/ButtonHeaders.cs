using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHeaders : MonoBehaviour
{
    public Button HomeButton;
    public Button PubChemButton;
    public Button BiotransformerButton;
    public Button SkippedCompoundButton;

    public GameObject HomePanel;
    public GameObject PubChemPanel;
    public GameObject BiotransformerPanel;
    public GameObject SkippedCompoundPanel;

    void Awake()
    {
        SwitchPanels(0);
        SkippedCompoundButton.gameObject.SetActive(false);
    }

    public void SwitchPanels(int panel)
    {
        ToggleAllPanels();

        switch (panel)
        {
            case 0:
                HomePanel.SetActive(true);
                break;
            case 1:
                PubChemPanel.SetActive(true);
                break;
            case 2:
                BiotransformerPanel.SetActive(true);
                break;
            case 3:
                SkippedCompoundPanel.SetActive(true);
                break;

        }
    }

    public void ToggleAllPanels()
    {
        HomePanel.SetActive(false);
        PubChemPanel.SetActive(false);
        BiotransformerPanel.SetActive(false);
        SkippedCompoundPanel.SetActive(false);
    }
}
