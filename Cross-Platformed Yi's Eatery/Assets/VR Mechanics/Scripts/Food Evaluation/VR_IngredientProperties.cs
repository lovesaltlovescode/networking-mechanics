using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using JetBrains.Annotations;
using System.Linq;

public interface I_VR_Ingredient
{
    VR_IngredientProperties Properties { get; }

}

public class VR_IngredientProperties : XRBaseInteractable
{
    //public getters
    public int StepNum
    {
        get { return stepNum; }
    }
    public bool NeedCooked
    {
        get { return needCooked; }
    }
    public bool NeedSliced
    {
        get { return needSliced; }
    }
    public bool CanCook
    {
        get { return canCook; }
    }
    public bool CanSlice
    {
        get { return canSlice; }
    }
    public bool CanMould
    {
        get { return canMould; }
    }


    //private setters //make another class for these properties? Kinda overkill :P
    private bool needCooked = false, needSliced = false;
    private bool canCook = false, canSlice = false, canMould = false;
    private bool cooked = false, sliced = false, moulded = false;
    private int stepNum = 0;

    public Action currentAction = null;

    [SerializeField] private PossibleIngredientType ingredientType = default;
    public PossibleIngredientType IngredientType { get { return ingredientType; } }

    public enum PossibleIngredientType
    {
        //ingredient types
        Chicken,
        Rice,
        Cucumber,
        Egg

    }

    //preparation steps
    [SerializeField] private VR_PreparationBase.PreparationType[] prepSteps;
    private VR_PreparationBase.PreparationType optionalStep = VR_PreparationBase.PreparationType.Moulding;

    public VR_IngredientProperties(VR_PreparationBase.PreparationType[] stepOrder)
    {
        this.prepSteps = stepOrder;
        this.SetBools(stepOrder);
    }


    #region listeners
    protected override void Awake()
    {
        base.Awake();
        onSelectExit.AddListener(EndPress);
    }

    private void OnDestroy()
    {
        onSelectExit.RemoveListener(EndPress);
    }
    #endregion

    private void EndPress(XRBaseInteractor interactor)
    {
        if(currentAction != null)
        {

        }
    }


        private void SetBools(VR_PreparationBase.PreparationType[] _stepOrder)
    {
        foreach (VR_PreparationBase.PreparationType prepType in _stepOrder)
        {
            switch (prepType)
            {
                case VR_PreparationBase.PreparationType.Cooking:
                    needCooked = true;
                    break;

                case VR_PreparationBase.PreparationType.Slicing:
                    needSliced = true;
                    break;

                default:
                    break;
            }
        }
    }




    public bool CheckOptional(VR_PreparationBase.PreparationType _prepType)
    {
        return _prepType == optionalStep;
    }

    public void NextStep()
    {
        if (stepNum + 1 < this.prepSteps.Count())
        {
            stepNum++;
        }
    }

    public int CheckCanPerformStep(VR_PreparationBase.PreparationType _prepType)
    {
        return Array.IndexOf(prepSteps, _prepType);
    }



    public void CookedSuccessfully()
    {
        cooked = true;
    }

    public void SlicedSuccessfully()
    {
        sliced = true;
    }

    public void MouldedSuccessfully()
    {
        moulded = true;
    }



    private bool CheckServable()
    {
        return (needCooked == cooked) && (needSliced == sliced);
    }
}






/*
public VR_PreparationBase.PreparationType CheckCurrentStep(VR_PreparationBase.PreparationType checkedStep)
{
    int stepNumOfCheckedStep = CheckCanPerformStep(checkedStep);

    if (stepNumOfCheckedStep < 0) //if checkedStep cannot be performed on the ingredient, do nothing
    {
        //the current step cannot be carried out at all on this ingredient, ever //need do == done
        //do nothing
        return default;
    }
    else if((stepNumOfCheckedStep == stepNum) || //if the checkedStep matches the current step
        (stepNumOfCheckedStep - stepNum == 1 && CheckOptional(prepSteps[stepNumOfCheckedStep - 1]))) //the prev step was optional
    {
        //yes the current step can be carried out //need do != done
        //glow green
        return checkedStep;
    } 
    else //step can be carried out, but at a different time
    {
        int differenceInSteps = stepNumOfCheckedStep - stepNum;
        Debug.Log("difference in step num " + differenceInSteps);

        if(differenceInSteps < 0)
        {
            //the current step was carried out before //need do == done
            //glow red
            return checkedStep;

        } 
        else if(stepNumOfCheckedStep - stepNum == 1 && CheckOptional(prepSteps[stepNumOfCheckedStep - 1]))
        {
            //the prev step was optional, so you can carry this step out //need do != done
            //glow green
            return checkedStep;
        }
        else
        {
            // it's too early to carry this step out
            //glow red, indicate steps to carry out //need do != done
            return prepSteps[stepNum];
        }
    }
}
*/
