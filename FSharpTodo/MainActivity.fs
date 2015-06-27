namespace FSharpTodo

open System
open System.Collections.Generic
open Android.App
open Android.Content
open Android.OS
open Android.Runtime
open Android.Views
open Android.Widget

[<Activity (Label = "F# Todo", Theme = "@android:style/Theme.Holo.Light", MainLauncher = true)>]
type MainActivity () =
    inherit Activity ()

    member val items : List<TodoItem> = new List<TodoItem>() with get, set
    member val allItems : ListView = null with get, set
    member val taskName : EditText = null with get, set
    member val addButton : Button = null with get, set
    member val errorMessage : TextView = null with get, set

    member this.UpdateData() = 
        this.items <- App.ItemManager.GetItems()
        let adapter = new ArrayAdapter<TodoItem>(this, Android.Resource.Layout.SimpleListItem1, this.items)
        this.allItems.Adapter <- adapter

    override this.OnCreate (bundle) =

        base.OnCreate (bundle)

        this.SetContentView (Resource_Layout.Main)

        this.taskName <- this.FindViewById<EditText>(Resource_Id.taskName)
        this.addButton <- this.FindViewById<Button>(Resource_Id.addButton)
        this.errorMessage <- this.FindViewById<TextView>(Resource_Id.errorMessage)
        this.allItems <- this.FindViewById<ListView>(Resource_Id.allItems)

        this.allItems.ItemClick.Add(fun args -> 
            let selectedItem = this.items.[args.Position]

            let i = new Intent(this, typeof<EditToDoActivity>)
            i.PutExtra("id", selectedItem.Id) |> ignore
            this.StartActivity(i) |> ignore
        )

        this.addButton.Click.Add (fun args -> 
            try
                App.ItemManager.AddItem(this.taskName.Text) |> ignore
            with 
                | :? Exception -> this.errorMessage.Text <- "There was a problem adding the item. Check that you've provided an item and there are no duplicates"
            this.taskName.Text <- ""
            this.UpdateData()
        )

    override this.OnResume() = 

        base.OnResume()

        this.UpdateData()
