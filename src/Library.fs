namespace BTable

open BTable.Table
open Feliz
open System

module Say =
  open Browser.Dom

  let root = ReactDOM.createRoot (document.getElementById "app")

  type Hi = {
    Song: string
    Artist: string
    Year: string
  }

  let items: Hi list = [
    {
      Song = "The Sliding Mr. Bones (Next Stop, Pottersville)"
      Artist = "Malcolm Lockyer"
      Year = "1961"
    }
    {
      Song = "Boom Boom Pow"
      Artist = "Black Eyed Ps"
      Year = "2001"
    }
  ]

  let itemToStrings (hi: Hi) : string list = [
    string hi.Song
    string hi.Artist
    string hi.Year
  ]

  let columns: Column<Hi> list = [
    {
      name = "Song"
      sorter = fun a b -> String.Compare(a.Song, b.Song)
    }
    {
      name = "Artist"
      sorter = fun a b -> String.Compare(a.Artist, b.Artist)
    }
    {
      name = "Year"
      sorter = fun a b -> String.Compare(a.Year, b.Year)
    }
  ]

  let w: Wrapper<Hi> = {
    items = items
    columns = columns
    itemToStrings = itemToStrings
  }

  root.render (Table w)
