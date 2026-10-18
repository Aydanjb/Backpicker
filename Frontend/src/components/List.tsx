import type { JSX } from "react";
import {
  Table,
  TableBody,
  TableCell,
  TableFooter,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import ListRow from "./ListRow";

type Props = {
  className?: string;
};

function List({ className }: Props): JSX.Element {
  return (
    <Table className={className}>
      <TableHeader>
        <TableRow>
          <TableHead>Item</TableHead>
          <TableHead>Selection</TableHead>
          <TableHead>Weight</TableHead>
          <TableHead>Price</TableHead>
          <TableHead>Where</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        <ListRow
          item="Backpack"
          weight="30 oz."
          price="$149.99"
          retailerLink="https://rei.com"
        ></ListRow>
        <ListRow
          item="Tent"
          weight="40 oz."
          price="$189.99"
          retailerLink="https://rei.com"
        ></ListRow>
        <ListRow
          item="Sleeping Pad"
          weight="12 oz."
          price="$39.99"
          retailerLink="https://rei.com"
        ></ListRow>
        <ListRow
          item="Pillow"
          weight="8 oz."
          price="$11.99"
          retailerLink="https://rei.com"
        ></ListRow>
      </TableBody>
      <TableFooter>
        <TableRow className="text-xl font-bold">
          <TableCell colSpan={2}>Total</TableCell>
          <TableCell>70 oz.</TableCell>
          <TableCell>$380.99</TableCell>
          <TableCell></TableCell>
        </TableRow>
      </TableFooter>
    </Table>
  );
}

export default List;
