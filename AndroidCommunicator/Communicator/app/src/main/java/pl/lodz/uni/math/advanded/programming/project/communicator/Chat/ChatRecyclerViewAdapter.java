package pl.lodz.uni.math.advanded.programming.project.communicator.Chat;

import android.content.Context;
import android.text.format.DateFormat;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.ValueEventListener;

import java.util.ArrayList;
import java.util.List;

import pl.lodz.uni.math.advanded.programming.project.communicator.R;

public class ChatRecyclerViewAdapter extends RecyclerView.Adapter<ChatRecyclerViewAdapter.ItemViewHolder> {
    private List<ChatMessage> messagesList = new ArrayList<>();
    private Context mContext;
    private DatabaseReference mRef;

    public ChatRecyclerViewAdapter(Context mContext, DatabaseReference ref) {
        this.mContext = mContext;
        this.mRef = ref;
        mRef.addValueEventListener(new ValueEventListener() {
            @Override
            public void onDataChange(@NonNull DataSnapshot dataSnapshot) {
                messagesList.clear();
                for (DataSnapshot postSnapshot : dataSnapshot.getChildren()) {
                    ChatMessage message = new ChatMessage();
                    String messageUser = postSnapshot.child("messageText").getValue().toString();
                    message.setMessageText(messageUser);

                    String messageTime = postSnapshot.child("messageTime").getValue().toString();
                    message.setMessageTime(Long.parseLong(messageTime));

                    String messageText = postSnapshot.child("messageUser").getValue().toString();
                    message.setMessageUser(messageText);


                    messagesList.add(message);
                }
                notifyDataSetChanged();
            }

            @Override
            public void onCancelled(@NonNull DatabaseError databaseError) {

            }
        });
    }

    @NonNull
    @Override
    public ChatRecyclerViewAdapter.ItemViewHolder onCreateViewHolder(@NonNull ViewGroup viewGroup, int i) {
        View view;
        LayoutInflater mInflater = LayoutInflater.from(mContext);
        view = mInflater.inflate(R.layout.message, viewGroup, false);

        return new ChatRecyclerViewAdapter.ItemViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull final ChatRecyclerViewAdapter.ItemViewHolder itemViewHolder, final int i) {
        itemViewHolder.userName.setText(messagesList.get(i).getMessageUser());
        itemViewHolder.messageText.setText(messagesList.get(i).getMessageText());
        itemViewHolder.messageTime.setText(DateFormat.format("dd-mm-yyyy (HH:mm:ss)", messagesList.get(i).getMessageTime()));

    }

    @Override
    public int getItemCount() {
        return messagesList.size();
    }

    public class ItemViewHolder extends RecyclerView.ViewHolder {
        TextView userName, messageText, messageTime;


        public ItemViewHolder(@NonNull View itemView) {
            super(itemView);
            userName = itemView.findViewById(R.id.message_user);
            messageText = itemView.findViewById(R.id.message_text);
            messageTime = itemView.findViewById(R.id.message_time);
        }

    }
}
