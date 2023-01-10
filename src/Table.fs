namespace BTable

open Elmish.Obsolete
open Fable.Core
open Feliz
open Feliz.UseElmish
open Elmish
open Fable.Core.JsInterop

module Table =
  type private Model =
    {
      headers: string list
      rows: string list list
      sortKey: string
      sortingOrder: SortingOrder
    }

    static member Init = {
      headers = []
      rows = []
      sortKey = ""
      sortingOrder = SortingOrder.Ascending
    }

  and [<RequireQualifiedAccess>] private SortingOrder =
    | Ascending
    | Descending

  [<RequireQualifiedAccess>]
  type private Msg = SortingOrderToggled of elementId: string

  let inline private init (headers: string list) (rows: string list list) =
    { Model.Init with
        headers = headers
        rows = rows
    },
    Cmd.none

  let private update msg (model: Model) =
    match msg with
    | Msg.SortingOrderToggled elementId ->
      let reverseSortingOrder currentOrder =
        match currentOrder with
        | SortingOrder.Ascending -> SortingOrder.Descending
        | SortingOrder.Descending -> SortingOrder.Ascending

      JS.console.log elementId

      { model with
          sortingOrder = reverseSortingOrder model.sortingOrder
          sortKey = elementId
      },
      Cmd.none

  let private view (model: Model) (dispatch: Msg Dispatch) =
    let tableHead =
      Html.thead [
        prop.classes [ tw.uppercase; tw.bg_gray_200 ]
        prop.children [
          Html.tr [
            prop.children [
              for s in [ "Song"; "Artist"; "Year" ] ->
                Html.th [
                  prop.scope "col"
                  prop.classes []
                  prop.children [
                    // In button to make both clickable with hand on hover
                    Html.button [
                      prop.id s
                      prop.onClick (fun e ->
                        e.currentTarget?id |> Msg.SortingOrderToggled |> dispatch)
                      prop.classes [ tw.flex; tw.w_full ]
                      prop.children [
                        Html.text s
                        Html.i [
                          prop.classes [
                            tw.px_2
                            fa.fa_solid
                            if model.sortKey = s then
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
        for row in model.rows ->
          Html.tr [
            prop.classes []
            prop.children [
              for cell in row ->
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
  let Table (headers: string list) (rows: string list list) =
    JsInterop.importAll "../index.css"

    let model, dispatch = React.useElmish (init (unbox headers) rows, update, [||])

    view model dispatch
