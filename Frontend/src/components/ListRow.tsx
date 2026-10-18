import { TableCell, TableRow } from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import REILogo from "@/assets/REI_idsW56ot2c_0.svg";
import { IconPlus } from "@tabler/icons-react";

type Props = {
  item: string;
  weight: string;
  price: string;
  retailerLink: string;
};

function ListRow({ item, weight, price, retailerLink }: Props) {
  return (
    <TableRow>
      <TableCell className="capitalize underline">
        <a href={"/products/" + item}>{item}</a>
      </TableCell>
      <TableCell>
        <Button>
          <IconPlus stroke={3} data-icon="inline-start" /> Add a {item}
        </Button>
      </TableCell>
      <TableCell>{weight}</TableCell>
      <TableCell className="font-bold text-green-600">{price}</TableCell>
      <TableCell>
        <div className="flex flex-row gap-6">
          <a href={retailerLink}>
            <img src={REILogo} alt="REI Logo" width="50" />
          </a>
          <a href={retailerLink}>
            <Button className="bg-green-600">Buy</Button>
          </a>
        </div>
      </TableCell>
    </TableRow>
  );
}

export default ListRow;
