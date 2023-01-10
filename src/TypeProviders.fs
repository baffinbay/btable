[<AutoOpen>]
module TypeProviders

open Zanaptak.TypedCssClasses

type tw =
  CssClasses<"../index.css", Naming.Underscores, commandFile="node", argumentPrefix="../tailwind.process.js ../tailwind.all.js">

type fa =
  CssClasses<"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.2.1/css/all.min.css", Naming.Underscores>
