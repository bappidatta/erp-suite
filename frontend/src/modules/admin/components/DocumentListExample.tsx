import { useState } from "react";
import { Button } from "@app/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  DialogFooter,
} from "@app/components/ui/dialog";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@app/components/ui/dropdown-menu";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@app/components/ui/card";
import { Badge } from "@app/components/ui/badge";
import { MoreVertical, Download, Trash2, Edit } from "lucide-react";

interface DocumentItem {
  id: string;
  name: string;
  type: string;
  date: string;
  status: "draft" | "approved" | "archived";
}

export function DocumentListExample() {
  const [isOpen, setIsOpen] = useState(false);
  const [selectedDoc, setSelectedDoc] = useState<DocumentItem | null>(null);

  const documents: DocumentItem[] = [
    { id: "1", name: "Invoice #INV-001", type: "Invoice", date: "Mar 8, 2026", status: "approved" },
    { id: "2", name: "PO #PO-042", type: "Purchase Order", date: "Mar 7, 2026", status: "draft" },
    { id: "3", name: "Receipt #REC-015", type: "Receipt", date: "Mar 6, 2026", status: "approved" },
  ];

  const getStatusColor = (status: string) => {
    switch (status) {
      case "approved":
        return "bg-green-100 text-green-800";
      case "draft":
        return "bg-yellow-100 text-yellow-800";
      case "archived":
        return "bg-gray-100 text-gray-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  const handleExport = (doc: DocumentItem) => {
    console.log("Exporting", doc.name);
    // Would export document
  };

  const handleEdit = (doc: DocumentItem) => {
    setSelectedDoc(doc);
    setIsOpen(true);
  };

  const handleDelete = (doc: DocumentItem) => {
    console.log("Deleting", doc.name);
    // Would delete document
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle>Documents</CardTitle>
        <CardDescription>Manage your business documents</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="space-y-3">
          {documents.map((doc) => (
            <div key={doc.id} className="flex items-center justify-between p-3 border rounded-lg hover:bg-accent">
              <div className="flex-1">
                <h4 className="font-medium">{doc.name}</h4>
                <p className="text-xs text-muted-foreground">{doc.type} • {doc.date}</p>
              </div>
              <div className="flex items-center gap-2">
                <Badge className={getStatusColor(doc.status)}>
                  {doc.status.charAt(0).toUpperCase() + doc.status.slice(1)}
                </Badge>

                {/* Dropdown Menu for Actions */}
                <DropdownMenu>
                  <DropdownMenuTrigger>
                    <Button variant="ghost" size="sm">
                      <MoreVertical className="h-4 w-4" />
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    <DropdownMenuLabel>Actions</DropdownMenuLabel>
                    <DropdownMenuSeparator />
                    <DropdownMenuItem onClick={() => handleExport(doc)}>
                      <Download className="h-4 w-4 mr-2" />
                      Download
                    </DropdownMenuItem>
                    <DropdownMenuItem onClick={() => handleEdit(doc)}>
                      <Edit className="h-4 w-4 mr-2" />
                      Edit
                    </DropdownMenuItem>
                    <DropdownMenuSeparator />
                    <DropdownMenuItem
                      onClick={() => handleDelete(doc)}
                      className="text-red-600"
                    >
                      <Trash2 className="h-4 w-4 mr-2" />
                      Delete
                    </DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </div>
            </div>
          ))}
        </div>
      </CardContent>

      {/* Dialog for Editing */}
      <Dialog open={isOpen} onOpenChange={setIsOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Edit Document</DialogTitle>
            <DialogDescription>
              Modify the details of {selectedDoc?.name}
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4 py-4">
            <div>
              <label className="text-sm font-medium">Document Name</label>
              <input
                type="text"
                defaultValue={selectedDoc?.name}
                className="w-full mt-1 px-3 py-2 border rounded-md"
              />
            </div>
            <div>
              <label className="text-sm font-medium">Status</label>
              <select className="w-full mt-1 px-3 py-2 border rounded-md">
                <option value="draft">Draft</option>
                <option value="approved">Approved</option>
                <option value="archived">Archived</option>
              </select>
            </div>
          </div>

          <DialogFooter>
            <Button variant="outline" onClick={() => setIsOpen(false)}>
              Cancel
            </Button>
            <Button onClick={() => setIsOpen(false)}>
              Save Changes
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </Card>
  );
}
