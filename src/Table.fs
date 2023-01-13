namespace BTable

open Elmish.Obsolete
open Fable.Core
open Feliz
open Feliz.UseElmish
open Elmish
open Fable.Core.JsInterop

module Table =
  type Wrapper<'a> = {
    items: 'a list
    columns: Column<'a> list
    itemToStrings: 'a -> string list
  }

  and Column<'a> = {
    name: string
    sorter: 'a -> 'a -> int
  }

  type private Model<'a> =
    {
      items: 'a list
      columns: Column<'a> list
      itemToStrings: 'a -> string list
      sortKey: string
      sortingOrder: SortingOrder
    }

    static member Init items columns itemsToStrings : Model<'a> = {
      items = items
      columns = columns
      itemToStrings = itemsToStrings
      sortKey = ""
      sortingOrder = SortingOrder.Ascending
    }

  and [<RequireQualifiedAccess>] private SortingOrder =
    | Ascending
    | Descending

  [<RequireQualifiedAccess>]
  type private Msg = SortingOrderToggled of elementId: string

  let inline private init<'a> (items: Wrapper<'a>) : Model<'a> * Cmd<Msg> =
    Model.Init items.items items.columns items.itemToStrings, Cmd.none

  let inline private update msg (model: Model<'a>) =
    match msg with
    | Msg.SortingOrderToggled elementId ->
      let reverseSortingOrder currentOrder =
        match currentOrder with
        | SortingOrder.Ascending -> SortingOrder.Descending
        | SortingOrder.Descending -> SortingOrder.Ascending

      JS.console.log elementId

      let col = model.columns |> List.find (fun c -> c.name = elementId)

      let sorted currentOrder =
        let s = model.items |> List.sortWith col.sorter

        match currentOrder with
        | SortingOrder.Ascending -> s
        | SortingOrder.Descending -> List.rev s

      { model with
          items = sorted model.sortingOrder
          sortingOrder = reverseSortingOrder model.sortingOrder
          sortKey = elementId
      },
      Cmd.none

  let private view (model: Model<'a>) (dispatch: Dispatch<Msg>) =
    let tableHead =
      Html.thead [
        prop.classes [ tw.uppercase; tw.bg_gray_200 ]
        prop.children [
          Html.tr [
            prop.children [
              for column in model.columns ->
                Html.th [
                  prop.scope "col"
                  prop.classes []
                  prop.children [
                    // In button to make both clickable with hand on hover
                    Html.button [
                      prop.id column.name
                      prop.onClick (fun e ->
                        e.currentTarget?id |> Msg.SortingOrderToggled |> dispatch)
                      prop.classes [ tw.flex; tw.w_full ]
                      prop.children [
                        Html.text column.name
                        Html.i [
                          prop.classes [
                            tw.px_2
                            fa.fa_solid
                            if model.sortKey = column.name then
                              match model.sortingOrder with
                              | SortingOrder.Ascending -> fa.fa_sort_up
                              | SortingOrder.Descending -> fa.fa_sort_down
                          ]
                        ]
                      ]
                    ]
                  ]
                ]
            ]
          ]
        ]
      ]

    let tableBody =
      Html.tbody [
        for item in model.items ->
          Html.tr [
            prop.classes []
            prop.children [
              for cell in model.itemToStrings item ->
                Html.td [ prop.classes []; prop.children [ Html.text cell ] ]
            ]
          ]
      ]

    Html.div [
      prop.classes [ tw.relative; tw.overflow_x_auto; tw.rounded_lg ]
      prop.children [
        Html.table [
          prop.classes [ tw.w_full; tw.text_left ]
          prop.children [ tableHead; tableBody ]
        ]
      ]
    ]

  [<ReactComponent>]
  let Table (items: Wrapper<'a>) =
    JsInterop.importAll "../index.css"

    let model, dispatch = React.useElmish (init items, update, [||])

    view model dispatch
