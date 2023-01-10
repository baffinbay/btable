namespace BTable

open Feliz

module Say =
  open Browser.Dom

  let root = ReactDOM.createRoot (document.getElementById "app")

  let hi = [
    [ "The Sliding Mr. Bones (Next Stop, Pottersville)"; "Malcolm Lockyer"; "1961" ]
    [ "Boom Boom Pow"; "Black Eyed Ps"; "2001" ]
  ]

  root.render (Table.Table [ "Song";"Artist";"Year" ] hi)
