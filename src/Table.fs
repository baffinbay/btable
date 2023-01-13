namespace BTable

open Fable.Core
open Feliz
open Feliz.UseElmish
open Elmish
open Fable.Core.JsInterop

module Table =
  type Config<'rowData> = {
    rowDataToStrings: 'rowData -> string list
    columns: Column<'rowData> list
  }

  and Column<'rowData> = {
    name: string
    sorter: 'rowData -> 'rowData -> int
  }

  type private Model<'rowData> =
    {
      rowData: 'rowData list
      columns: Column<'rowData> list
      rowDataToStrings: 'rowData -> string list
      sortKey: string
      sortingOrder: SortingOrder
    }

    static member Init rowData columns rowDataToStrings : Model<'rowData> = {
      rowData = rowData
      columns = columns
      rowDataToStrings = rowDataToStrings
      sortKey = ""
      sortingOrder = SortingOrder.Ascending
    }

  and [<RequireQualifiedAccess>] private SortingOrder =
    | Ascending
    | Descending

  [<RequireQualifiedAccess>]
  type private Msg = SortingOrderToggled of elementId: string

  let inline private init<'rowData>
    (config: Config<'rowData>)
    (rowData: 'rowData list)
    : Model<'rowData> * Cmd<Msg> =
    Model.Init rowData config.columns config.rowDataToStrings, Cmd.none

  let inline private update msg (model: Model<'a>) =
    match msg with
    | Msg.SortingOrderToggled elementId ->
      JS.console.log elementId
      // This lookup should always work.
      let col = model.columns |> List.find (fun c -> c.name = elementId)

      let sortedRowData, newSortingOrder =
        let sorted = model.rowData |> List.sortWith col.sorter

        match model.sortingOrder with
        | SortingOrder.Ascending -> sorted, SortingOrder.Descending
        | SortingOrder.Descending -> List.rev sorted, SortingOrder.Ascending

      { model with
          rowData = sortedRowData
          sortingOrder = newSortingOrder
          sortKey = elementId
      },
      Cmd.none

  let private view (model: Model<_>) (dispatch: Dispatch<Msg>) =
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
        for item in model.rowData ->
          Html.tr [
            prop.classes []
            prop.children [
              for cell in model.rowDataToStrings item ->
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
  let Table<'rowData> (config: Config<'rowData>) (rowData: 'rowData list) =
    JsInterop.importAll "../index.css"

    let model, dispatch = React.useElmish (init config rowData, update, [||])

    view model dispatch
